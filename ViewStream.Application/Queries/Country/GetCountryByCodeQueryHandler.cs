using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Country
{
    public class GetCountryByCodeQueryHandler : IRequestHandler<GetCountryByCodeQuery, CountryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCountryByCodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CountryDto?> Handle(GetCountryByCodeQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Countries.FindAsync(
                predicate: c => c.Code == request.Code,
                include: q => q.Include(c => c.ShowAvailabilities),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var country = countries.FirstOrDefault();
            return country == null ? null : _mapper.Map<CountryDto>(country);
        }
    }
}
