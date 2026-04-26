using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.UpdateCountry
{
    using Country = ViewStream.Domain.Entities.Country;
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateCountryCommandHandler> _logger;

        public UpdateCountryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateCountryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating country with Code: {Code}", request.Code);

            var country = await _unitOfWork.Countries.GetByIdAsync<string>(request.Code, cancellationToken);
            if (country == null)
            {
                _logger.LogWarning("Country not found with Code: {Code}", request.Code);
                return false;
            }

            var oldValues = _mapper.Map<CountryDto>(country);
            _mapper.Map(request.Dto, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Country, object>(
                tableName: "Countries",
                recordId: country.Code.GetHashCode(),
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Country updated with Code: {Code}", country.Code);
            return true;
        }
    }
}
