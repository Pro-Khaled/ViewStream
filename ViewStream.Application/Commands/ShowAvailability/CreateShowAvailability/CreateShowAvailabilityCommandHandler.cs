using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability
{
    using ShowAvailability = Domain.Entities.ShowAvailability;
    public class CreateShowAvailabilityCommandHandler : IRequestHandler<CreateShowAvailabilityCommand, (long ShowId, string CountryCode)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateShowAvailabilityCommandHandler> _logger;

        public CreateShowAvailabilityCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateShowAvailabilityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<(long ShowId, string CountryCode)> Handle(CreateShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating availability for ShowId: {ShowId}, CountryCode: {CountryCode}",
                request.Dto.ShowId, request.Dto.CountryCode);

            var availability = _mapper.Map<ShowAvailability>(request.Dto);
            await _unitOfWork.ShowAvailabilities.AddAsync(availability, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ShowAvailability, object>(
                tableName: "ShowAvailabilities",
                recordId: availability.ShowId.GetHashCode() ^ availability.CountryCode.GetHashCode(),
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Availability created: ShowId={ShowId}, CountryCode={CountryCode}",
                availability.ShowId, availability.CountryCode);
            return (availability.ShowId, availability.CountryCode);
        }
    }
}
