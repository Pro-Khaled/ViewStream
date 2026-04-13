using AutoMapper;
using MediatR;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.CreateEpisode
{
    using Episode = Domain.Entities.Episode;
    public class CreateEpisodeCommandHandler : IRequestHandler<CreateEpisodeCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;


        public CreateEpisodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<long> Handle(CreateEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = _mapper.Map<Episode>(request.Dto);
            episode.CreatedAt = DateTime.UtcNow;
            episode.IsDeleted = false;

            await _unitOfWork.Episodes.AddAsync(episode, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Handle video file upload if present
            if (request.Dto.VideoFile != null)
            {
                var videoUrl = await _fileStorage.SaveVideoAsync(request.Dto.VideoFile, episode.Id, cancellationToken);
                episode.VideoUrl = videoUrl;
                _unitOfWork.Episodes.Update(episode);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return episode.Id;
        }
    }
}
