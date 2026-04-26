using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.CreateSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class CreateSharedListCommandHandler : IRequestHandler<CreateSharedListCommand, SharedListDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateSharedListCommandHandler> _logger;

        public CreateSharedListCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateSharedListCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<SharedListDto> Handle(CreateSharedListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating shared list '{ListName}' for ProfileId: {ProfileId}",
                request.Dto.Name, request.OwnerProfileId);

            var list = new SharedList
            {
                OwnerProfileId = request.OwnerProfileId,
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                IsPublic = request.Dto.IsPublic ?? false,
                ShareCode = GenerateUniqueShareCode(),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SharedLists.AddAsync(list, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<SharedList, object>(
                tableName: "SharedLists",
                recordId: list.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { list.Name, list.Description, list.IsPublic, list.OwnerProfileId },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Shared list created with Id: {ListId}", list.Id);

            var result = await _unitOfWork.SharedLists.FindAsync(
                l => l.Id == list.Id,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListDto>(result.First());
        }

        private string GenerateUniqueShareCode()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 12);
        }
    }
}
