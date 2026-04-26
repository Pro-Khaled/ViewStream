using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.UpdateContentTag
{
    using ContentTag = ViewStream.Domain.Entities.ContentTag;
    public class UpdateContentTagCommandHandler : IRequestHandler<UpdateContentTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateContentTagCommandHandler> _logger;

        public UpdateContentTagCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateContentTagCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateContentTagCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating content tag with Id: {TagId}", request.Id);

            var tag = await _unitOfWork.ContentTags.GetByIdAsync<int>(request.Id, cancellationToken);
            if (tag == null)
            {
                _logger.LogWarning("Content tag not found with Id: {TagId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ContentTagDto>(tag);
            _mapper.Map(request.Dto, tag);
            _unitOfWork.ContentTags.Update(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Positional arguments
            _auditContext.SetAudit<ContentTag, UpdateContentTagDto>(
                "ContentTags",
                tag.Id,
                "UPDATE",
                oldValues,
                request.Dto,
                request.UserId
            );

            _logger.LogInformation("Content tag updated with Id: {TagId}", tag.Id);
            return true;
        }
    }
}
