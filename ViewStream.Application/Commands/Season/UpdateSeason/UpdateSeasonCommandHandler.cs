using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.UpdateSeason
{
    public class UpdateSeasonCommandHandler : IRequestHandler<UpdateSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSeasonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted == true) return false;

            _mapper.Map(request.Dto, season);
            season.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Seasons.Update(season);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
