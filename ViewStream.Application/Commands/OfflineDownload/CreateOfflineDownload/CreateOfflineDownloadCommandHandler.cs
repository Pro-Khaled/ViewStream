using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload
{
    using OfflineDownload = ViewStream.Domain.Entities.OfflineDownload;
    public class CreateOfflineDownloadCommandHandler : IRequestHandler<CreateOfflineDownloadCommand, OfflineDownloadDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateOfflineDownloadCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OfflineDownloadDto> Handle(CreateOfflineDownloadCommand request, CancellationToken cancellationToken)
        {
            var download = new OfflineDownload
            {
                ProfileId = request.ProfileId,
                EpisodeId = request.Dto.EpisodeId,
                DeviceId = request.Dto.DeviceId,
                DownloadQuality = request.Dto.DownloadQuality,
                FilePath = request.Dto.FilePath,
                DownloadedAt = DateTime.UtcNow,
                ExpiresAt = request.Dto.ExpiresAt ?? DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.OfflineDownloads.AddAsync(download, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.OfflineDownloads.FindAsync(
                d => d.Id == download.Id,
                include: q => q.Include(d => d.Episode).Include(d => d.Device),
                cancellationToken: cancellationToken);

            return _mapper.Map<OfflineDownloadDto>(result.First());
        }
    }
}
