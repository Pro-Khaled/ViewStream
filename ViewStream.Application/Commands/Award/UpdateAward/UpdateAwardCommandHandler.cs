using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.UpdateAward
{
    using Award = ViewStream.Domain.Entities.Award;

    public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, AwardDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateAwardCommandHandler> _logger;

        public UpdateAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<AwardDto?> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
        {
            var award = await _unitOfWork.Awards.GetByIdAsync<int>(request.Id, cancellationToken);
            if (award == null)
            {
                _logger.LogWarning("Attempt to update non-existent award Id: {Id}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<AwardDto>(award);
            _mapper.Map(request.Dto, award);

            _unitOfWork.Awards.Update(award);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Award, UpdateAwardDto>(
                tableName: "Awards",
                recordId: award.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UpdatedByUserId
            );

            _logger.LogInformation("Award updated with Id: {AwardId}", award.Id);
            return _mapper.Map<AwardDto>(award);
        }
    }
}
