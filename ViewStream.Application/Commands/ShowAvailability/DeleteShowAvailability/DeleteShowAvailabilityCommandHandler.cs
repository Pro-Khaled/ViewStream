using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability
{
    using ShowAvailability = ViewStream.Domain.Entities.ShowAvailability;
    public class DeleteShowAvailabilityCommandHandler : IRequestHandler<DeleteShowAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteShowAvailabilityCommandHandler> _logger;

        public DeleteShowAvailabilityCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteShowAvailabilityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting availability for ShowId: {ShowId}, CountryCode: {CountryCode}",
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
            _unitOfWork.ShowAvailabilities.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ShowAvailability, object>(
                tableName: "ShowAvailabilities",
                recordId: request.ShowId.GetHashCode() ^ request.CountryCode.GetHashCode(),
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Availability deleted: ShowId={ShowId}, CountryCode={CountryCode}",
                request.ShowId, request.CountryCode);
            return true;
        }
    }
}
