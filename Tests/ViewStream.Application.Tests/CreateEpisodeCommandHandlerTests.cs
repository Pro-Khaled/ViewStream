using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ViewStream.Application.Commands.Episode.CreateEpisode;
using ViewStream.Application.DTOs;
using ViewStream.Application.Contracts;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Tests
{
    using Episode = ViewStream.Domain.Entities.Episode;

    public class CreateEpisodeCommandHandlerTests
    {
        private readonly Mock<IEpisodeRepository> _episodeRepoMock;
        private readonly Mock<IVideoProcessingJobRepository> _jobRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileStorageService> _fileStorageMock;
        private readonly Mock<IAuditContext> _auditContextMock;
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly Mock<ILogger<CreateEpisodeCommandHandler>> _loggerMock;
        private readonly CreateEpisodeCommandHandler _handler;

        public CreateEpisodeCommandHandlerTests()
        {
            _episodeRepoMock = new Mock<IEpisodeRepository>();
            _jobRepoMock = new Mock<IVideoProcessingJobRepository>();
            
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.SetupGet(u => u.Episodes).Returns(_episodeRepoMock.Object);
            _unitOfWorkMock.SetupGet(u => u.VideoProcessingJobs).Returns(_jobRepoMock.Object);

            _mapperMock = new Mock<IMapper>();
            _fileStorageMock = new Mock<IFileStorageService>();
            _auditContextMock = new Mock<IAuditContext>();
            _messageBusMock = new Mock<IMessageBus>();
            _loggerMock = new Mock<ILogger<CreateEpisodeCommandHandler>>();

            _handler = new CreateEpisodeCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _fileStorageMock.Object,
                _auditContextMock.Object,
                _messageBusMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithVideoFile_ShouldUploadVideoAndCreateProcessingJob()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.Length).Returns(5000);
            formFileMock.Setup(f => f.FileName).Returns("test_video.mp4");

            var dto = new CreateEpisodeDto
            {
                SeasonId = 1,
                EpisodeNumber = 1,
                Title = "Episode 1",
                Description = "A great episode",
                RuntimeSeconds = 1200,
                VideoUrl = "temp_url",
                VideoFile = formFileMock.Object
            };

            var command = new CreateEpisodeCommand(dto, CreatedByUserId: 100);

            _mapperMock.Setup(m => m.Map<Episode>(dto))
                .Returns(new Episode
                {
                    SeasonId = dto.SeasonId,
                    EpisodeNumber = dto.EpisodeNumber,
                    Title = dto.Title,
                    Description = dto.Description,
                    RuntimeSeconds = dto.RuntimeSeconds
                });

            // Mock first storage call (returns null or empty to test the ID-based fallback upload path)
            _fileStorageMock.Setup(s => s.SaveVideoAsync(formFileMock.Object, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            // Mock second storage call (using episode.Id)
            _fileStorageMock.Setup(s => s.SaveVideoAsync(formFileMock.Object, 456, It.IsAny<CancellationToken>()))
                .ReturnsAsync("uploads/episodes/456/test_video.mp4");

            // Mock repository additions
            _episodeRepoMock.Setup(r => r.AddAsync(It.IsAny<Episode>(), It.IsAny<CancellationToken>()))
                .Callback<Episode, CancellationToken>((e, c) => e.Id = 456)
                .Returns(Task.CompletedTask);

            _jobRepoMock.Setup(r => r.GetPendingByEpisodeIdAsync(456))
                .ReturnsAsync((VideoProcessingJob?)null);

            _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<VideoProcessingJob>(), It.IsAny<CancellationToken>()))
                .Callback<VideoProcessingJob, CancellationToken>((j, c) => j.Id = 789)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(456);

            // Verify both file storage calls
            _fileStorageMock.Verify(s => s.SaveVideoAsync(formFileMock.Object, 0, It.IsAny<CancellationToken>()), Times.Once);
            _fileStorageMock.Verify(s => s.SaveVideoAsync(formFileMock.Object, 456, It.IsAny<CancellationToken>()), Times.Once);

            // Verify Episode creation and updates
            _episodeRepoMock.Verify(r => r.AddAsync(It.Is<Episode>(e => e.Id == 456), It.IsAny<CancellationToken>()), Times.Once);
            _episodeRepoMock.Verify(r => r.Update(It.Is<Episode>(e => e.VideoUrl == "uploads/episodes/456/test_video.mp4")), Times.Once);

            // Verify job creation
            _jobRepoMock.Verify(r => r.AddAsync(It.Is<VideoProcessingJob>(j => j.EpisodeId == 456 && j.SourceFileUrl == "uploads/episodes/456/test_video.mp4"), It.IsAny<CancellationToken>()), Times.Once);

            // Verify message bus publishing
            _messageBusMock.Verify(m => m.Publish(It.Is<TranscodeVideoMessage>(msg => msg.JobId == 789)), Times.Once);

            // Verify Auditing
            _auditContextMock.VerifySet(a => a.TableName = "Episodes", Times.Once);
            _auditContextMock.VerifySet(a => a.RecordId = 456, Times.Once);
            _auditContextMock.VerifySet(a => a.Action = "INSERT", Times.Once);
            _auditContextMock.VerifySet(a => a.ChangedByUserId = 100, Times.Once);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }

        [Fact]
        public async Task Handle_WithoutVideoFile_ShouldUseProvidedVideoUrlAndTriggerJob()
        {
            // Arrange
            var dto = new CreateEpisodeDto
            {
                SeasonId = 1,
                EpisodeNumber = 2,
                Title = "Episode 2",
                VideoUrl = "https://external-storage.com/episode2.mp4"
            };

            var command = new CreateEpisodeCommand(dto, CreatedByUserId: 100);

            _mapperMock.Setup(m => m.Map<Episode>(dto))
                .Returns(new Episode
                {
                    SeasonId = dto.SeasonId,
                    EpisodeNumber = dto.EpisodeNumber,
                    Title = dto.Title,
                    VideoUrl = dto.VideoUrl
                });

            _episodeRepoMock.Setup(r => r.AddAsync(It.IsAny<Episode>(), It.IsAny<CancellationToken>()))
                .Callback<Episode, CancellationToken>((e, c) => e.Id = 457)
                .Returns(Task.CompletedTask);

            _jobRepoMock.Setup(r => r.GetPendingByEpisodeIdAsync(457))
                .ReturnsAsync((VideoProcessingJob?)null);

            _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<VideoProcessingJob>(), It.IsAny<CancellationToken>()))
                .Callback<VideoProcessingJob, CancellationToken>((j, c) => j.Id = 790)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(457);

            // Should NOT upload file
            _fileStorageMock.Verify(s => s.SaveVideoAsync(It.IsAny<IFormFile>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);

            // Verify Episode creation
            _episodeRepoMock.Verify(r => r.AddAsync(It.Is<Episode>(e => e.Id == 457 && e.VideoUrl == "https://external-storage.com/episode2.mp4"), It.IsAny<CancellationToken>()), Times.Once);

            // Verify job creation
            _jobRepoMock.Verify(r => r.AddAsync(It.Is<VideoProcessingJob>(j => j.EpisodeId == 457 && j.SourceFileUrl == "https://external-storage.com/episode2.mp4"), It.IsAny<CancellationToken>()), Times.Once);

            // Verify message bus publishing
            _messageBusMock.Verify(m => m.Publish(It.Is<TranscodeVideoMessage>(msg => msg.JobId == 790)), Times.Once);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
