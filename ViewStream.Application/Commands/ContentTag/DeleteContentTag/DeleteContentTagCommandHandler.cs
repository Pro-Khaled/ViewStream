using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.DeleteContentTag
{
    using ContentTag = ViewStream.Domain.Entities.ContentTag;
    public class DeleteContentTagCommandHandler : IRequestHandler<DeleteContentTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteContentTagCommandHandler> _logger;

        public DeleteContentTagCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteContentTagCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteContentTagCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting content tag with Id: {TagId}", request.Id);

            var tag = await _unitOfWork.ContentTags.GetByIdAsync<int>(request.Id, cancellationToken);
            if (tag == null)
            {
                _logger.LogWarning("Content tag not found with Id: {TagId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ContentTagDto>(tag);
            _unitOfWork.ContentTags.Delete(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ContentTag, object>(
                tableName: "ContentTags",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Content tag deleted with Id: {TagId}", request.Id);
            return true;
        }
    }
}
