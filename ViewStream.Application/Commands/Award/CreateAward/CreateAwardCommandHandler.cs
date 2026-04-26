using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.CreateAward
{
    using Award = ViewStream.Domain.Entities.Award;

    public class CreateAwardCommandHandler : IRequestHandler<CreateAwardCommand, AwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateAwardCommandHandler> _logger;

        public CreateAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<AwardDto> Handle(CreateAwardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating award: {Name}", request.Dto.Name);

            var award = _mapper.Map<Award>(request.Dto);

            await _unitOfWork.Awards.AddAsync(award, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Award, CreateAwardDto>(
                tableName: "Awards",
                recordId: award.Id,
                action: "INSERT",
                newValues: request.Dto,
                changedByUserId: request.CreatedByUserId
            );

            _logger.LogInformation("Award created with Id: {AwardId}", award.Id);
            return _mapper.Map<AwardDto>(award);
        }
    }
}
