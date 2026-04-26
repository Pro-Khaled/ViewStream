using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.UpdateSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class UpdateSharedListCommandHandler : IRequestHandler<UpdateSharedListCommand, SharedListDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateSharedListCommandHandler> _logger;

        public UpdateSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<SharedListDto?> Handle(UpdateSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating shared list Id: {ListId}", request.Id);

            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
            {
                _logger.LogWarning("List not found or access denied. Id: {ListId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<SharedListDto>(list);
            list.Name = request.Dto.Name;
            list.Description = request.Dto.Description;
            list.IsPublic = request.Dto.IsPublic;

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedList, object>(
                tableName: "SharedLists",
                recordId: list.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { list.Name, list.Description, list.IsPublic },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Shared list updated. Id: {ListId}", list.Id);

            var result = await _unitOfWork.SharedLists.FindAsync(
                l => l.Id == list.Id,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListDto>(result.First());
        }
    }
}
