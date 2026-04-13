using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.CreateSeason
{
        using Season = Domain.Entities.Season;
    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSeasonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = _mapper.Map<Season>(request.Dto);
            season.CreatedAt = DateTime.UtcNow;
            season.IsDeleted = false;

            await _unitOfWork.Seasons.AddAsync(season, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return season.Id;
        }
    }
}
