using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability
{
    using ShowAvailability = ViewStream.Domain.Entities.ShowAvailability;

    public class UpdateShowAvailabilityCommandHandler : IRequestHandler<UpdateShowAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateShowAvailabilityCommandHandler> _logger;

        public UpdateShowAvailabilityCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateShowAvailabilityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating availability for ShowId: {ShowId}, CountryCode: {CountryCode}",
                request.ShowId, request.CountryCode);

            var availabilities = await _unitOfWork.ShowAvailabilities.FindAsync(
                sa => sa.ShowId == request.ShowId && sa.CountryCode == request.CountryCode,
                cancellationToken: cancellationToken);

            var entity = availabilities.FirstOrDefault();
            if (entity == null)
            {
                _logger.LogWarning("Availability not found: ShowId={ShowId}, CountryCode={CountryCode}",
                    request.ShowId, request.CountryCode);
                return false;
            }

            var oldValues = _mapper.Map<ShowAvailabilityDto>(entity);
            _mapper.Map(request.Dto, entity);
            _unitOfWork.ShowAvailabilities.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ShowAvailability, object>(
                tableName: "ShowAvailabilities",
                recordId: entity.ShowId.GetHashCode() ^ entity.CountryCode.GetHashCode(),
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Availability updated: ShowId={ShowId}, CountryCode={CountryCode}",
                request.ShowId, request.CountryCode);
            return true;
        }
    }
}
