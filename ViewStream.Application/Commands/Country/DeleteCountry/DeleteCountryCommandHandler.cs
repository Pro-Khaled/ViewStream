using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.DeleteCountry
{
    using Country = ViewStream.Domain.Entities.Country;
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteCountryCommandHandler> _logger;

        public DeleteCountryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteCountryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting country with Code: {Code}", request.Code);

            var country = await _unitOfWork.Countries.GetByIdAsync<string>(request.Code, cancellationToken);
            if (country == null)
            {
                _logger.LogWarning("Country not found with Code: {Code}", request.Code);
                return false;
            }

            var oldValues = _mapper.Map<CountryDto>(country);
            _unitOfWork.Countries.Delete(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Country, object>(
                tableName: "Countries",
                recordId: request.Code.GetHashCode(),
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Country deleted with Code: {Code}", request.Code);
            return true;
        }
    }
}
