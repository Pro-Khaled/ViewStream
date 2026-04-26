using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.DeleteAward
{
    public class DeleteAwardCommandHandler : IRequestHandler<DeleteAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteAwardCommandHandler> _logger;

        public DeleteAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeleteAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAwardCommand request, CancellationToken cancellationToken)
        {
            var award = await _unitOfWork.Awards.GetByIdAsync<int>(request.Id, cancellationToken);
            if (award == null)
            {
                _logger.LogWarning("Attempt to delete non-existent award Id: {Id}", request.Id);
                return false;
            }

            var oldValues = new
            {
                award.Id,
                // include any existing award fields relevant for audit
            };

            // perform physical delete because Award entity lacks soft-delete columns
            _unitOfWork.Awards.Delete(award);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<object, object>(
                tableName: "Awards",
                recordId: award.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.DeletedByUserId
            );

            _logger.LogInformation("Award deleted with Id: {AwardId}", award.Id);
            return true;
        }
    }
}
