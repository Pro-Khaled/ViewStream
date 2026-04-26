using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.CreateContentTag
{
    using ContentTag = Domain.Entities.ContentTag;

    public class CreateContentTagCommandHandler : IRequestHandler<CreateContentTagCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateContentTagCommandHandler> _logger;

        public CreateContentTagCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateContentTagCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(CreateContentTagCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating content tag: {TagName}, Category: {Category}",
                request.Dto.Name, request.Dto.Category);

            var tag = _mapper.Map<ContentTag>(request.Dto);
            await _unitOfWork.ContentTags.AddAsync(tag, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ContentTag, CreateContentTagDto>(
                tableName: "ContentTags",
                recordId: tag.Id,
                action: "INSERT",
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Content tag created with Id: {TagId}", tag.Id);
            return tag.Id;
        }
    }
}
