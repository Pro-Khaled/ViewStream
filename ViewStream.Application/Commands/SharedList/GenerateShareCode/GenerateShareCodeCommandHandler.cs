using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.GenerateShareCode
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class GenerateShareCodeCommandHandler : IRequestHandler<GenerateShareCodeCommand, string?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<GenerateShareCodeCommandHandler> _logger;

        public GenerateShareCodeCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<GenerateShareCodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string?> Handle(GenerateShareCodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating new share code for ListId: {ListId}", request.Id);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
            {
                _logger.LogWarning("List not found or access denied. Id: {ListId}", request.Id);
                return null;
            }

            var oldCode = list.ShareCode;
            list.ShareCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 12);

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedList, object>(
                tableName: "SharedLists",
                recordId: list.Id,
                action: "UPDATE",
                oldValues: new { ShareCode = oldCode },
                newValues: new { ShareCode = list.ShareCode },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("New share code generated for ListId: {ListId}", list.Id);
            return list.ShareCode;
        }
    }
}
