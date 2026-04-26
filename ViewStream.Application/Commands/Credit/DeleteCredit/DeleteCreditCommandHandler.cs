using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.DeleteCredit
{
    using Credit = ViewStream.Domain.Entities.Credit;
    public class DeleteCreditCommandHandler : IRequestHandler<DeleteCreditCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteCreditCommandHandler> _logger;

        public DeleteCreditCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteCreditCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCreditCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting credit with Id: {CreditId}", request.Id);

            var credit = await _unitOfWork.Credits.GetByIdAsync<long>(request.Id, cancellationToken);
            if (credit == null)
            {
                _logger.LogWarning("Credit not found with Id: {CreditId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<CreditDto>(credit);
            _unitOfWork.Credits.Delete(credit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Credit, object>(
                tableName: "Credits",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Credit deleted with Id: {CreditId}", request.Id);
            return true;
        }
    }
}
