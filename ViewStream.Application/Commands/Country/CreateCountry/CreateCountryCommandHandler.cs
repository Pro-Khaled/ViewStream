using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.CreateCountry
{
    using Country = Domain.Entities.Country;
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCountryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = _mapper.Map<Country>(request.Dto);
            await _unitOfWork.Countries.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return country.Code;
        }
    }
}
