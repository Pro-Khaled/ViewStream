using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PushToken.DeletePushToken
{
    using PushToken = ViewStream.Domain.Entities.PushToken;
    public class DeletePushTokenCommandHandler : IRequestHandler<DeletePushTokenCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePushTokenCommandHandler> _logger;

        public DeletePushTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeletePushTokenCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePushTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting push token Id: {TokenId} for UserId: {UserId}", request.Id, request.UserId);

            var token = await _unitOfWork.PushTokens.GetByIdAsync<long>(request.Id, cancellationToken);
            if (token == null || token.UserId != request.UserId) return false;

            _unitOfWork.PushTokens.Delete(token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PushToken, object>(
                tableName: "PushTokens",
                recordId: request.Id,
                action: "DELETE",
                oldValues: new { token.DeviceId, token.Platform },
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Push token deleted. Id: {TokenId}", request.Id);
            return true;
        }
    }
}
