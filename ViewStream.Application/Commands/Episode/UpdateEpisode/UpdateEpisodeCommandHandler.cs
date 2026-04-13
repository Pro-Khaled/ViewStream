using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.UpdateEpisode
{
    public class UpdateEpisodeCommandHandler : IRequestHandler<UpdateEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEpisodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted == true)
                return false;

            _mapper.Map(request.Dto, episode);
            episode.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Episodes.Update(episode);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
