using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.CreateCountry
{
    using Country = Domain.Entities.Country;
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateCountryCommandHandler> _logger;

        public CreateCountryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateCountryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating country with Code: {Code}, Name: {Name}",
                request.Dto.Code, request.Dto.Name);

            var country = _mapper.Map<Country>(request.Dto);
            await _unitOfWork.Countries.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Country, object>(
                tableName: "Countries",
                recordId: country.Code.GetHashCode(),
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Country created with Code: {Code}", country.Code);
            return country.Code;
        }
    }
}
