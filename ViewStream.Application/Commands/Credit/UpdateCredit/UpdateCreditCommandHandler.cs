using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.UpdateCredit
{
    using Credit = ViewStream.Domain.Entities.Credit;
    public class UpdateCreditCommandHandler : IRequestHandler<UpdateCreditCommand, CreditDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateCreditCommandHandler> _logger;

        public UpdateCreditCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateCreditCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<CreditDto?> Handle(UpdateCreditCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating credit with Id: {CreditId}", request.Id);

            var credit = await _unitOfWork.Credits.GetByIdAsync<long>(request.Id, cancellationToken);
            if (credit == null)
            {
                _logger.LogWarning("Credit not found with Id: {CreditId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<CreditDto>(credit);
            _mapper.Map(request.Dto, credit);
            _unitOfWork.Credits.Update(credit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Credit, object>(
                tableName: "Credits",
                recordId: credit.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Credit updated with Id: {CreditId}", credit.Id);

            var result = await _unitOfWork.Credits.FindAsync(
                c => c.Id == credit.Id,
                include: q => q.Include(c => c.Person)
                               .Include(c => c.Show)
                               .Include(c => c.Season).ThenInclude(s => s.Show)
                               .Include(c => c.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);
            return _mapper.Map<CreditDto>(result.First());
        }
    }
}
