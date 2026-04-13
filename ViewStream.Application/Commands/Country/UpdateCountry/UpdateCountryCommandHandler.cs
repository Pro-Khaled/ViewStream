using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.UpdateCountry
{
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCountryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync<string>(request.Code, cancellationToken);
            if (country == null) return false;

            _mapper.Map(request.Dto, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
