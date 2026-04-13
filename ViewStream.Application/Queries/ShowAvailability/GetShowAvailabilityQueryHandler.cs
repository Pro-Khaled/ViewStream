using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public class GetShowAvailabilityQueryHandler : IRequestHandler<GetShowAvailabilityQuery, ShowAvailabilityDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetShowAvailabilityQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShowAvailabilityDto?> Handle(GetShowAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var availabilities = await _unitOfWork.ShowAvailabilities.FindAsync(
                predicate: sa => sa.ShowId == request.ShowId && sa.CountryCode == request.CountryCode,
                include: q => q.Include(sa => sa.Show).Include(sa => sa.CountryCodeNavigation),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var availability = availabilities.FirstOrDefault();
            return availability == null ? null : _mapper.Map<ShowAvailabilityDto>(availability);
        }
    }
}
