using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public class GetAvailabilitiesByShowQueryHandler : IRequestHandler<GetAvailabilitiesByShowQuery, List<ShowAvailabilityListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAvailabilitiesByShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ShowAvailabilityListItemDto>> Handle(GetAvailabilitiesByShowQuery request, CancellationToken cancellationToken)
        {
            var availabilities = await _unitOfWork.ShowAvailabilities.FindAsync(
                predicate: sa => sa.ShowId == request.ShowId,
                include: q => q.Include(sa => sa.Show).Include(sa => sa.CountryCodeNavigation),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<ShowAvailabilityListItemDto>>(availabilities.OrderBy(sa => sa.CountryCodeNavigation.Name));
        }
    }
}
