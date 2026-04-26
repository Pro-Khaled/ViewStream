using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    using UserVector = ViewStream.Domain.Entities.UserVector;
    public class UpsertUserVectorCommandHandler : IRequestHandler<UpsertUserVectorCommand, UserVectorDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertUserVectorCommandHandler> _logger;

        public UpsertUserVectorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpsertUserVectorCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<UserVectorDto> Handle(UpsertUserVectorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting user vector for ProfileId: {ProfileId}", request.ProfileId);

            var vector = await _unitOfWork.UserVectors.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            bool isNew = vector == null;
            string? oldEmbedding = vector?.EmbeddingJson;

            if (isNew)
            {
                vector = new UserVector
                {
                    ProfileId = request.ProfileId,
                    EmbeddingJson = request.Dto.EmbeddingJson,
                    LastUpdated = DateTime.UtcNow
                };
                await _unitOfWork.UserVectors.AddAsync(vector, cancellationToken);
            }
            else
            {
                vector.EmbeddingJson = request.Dto.EmbeddingJson;
                vector.LastUpdated = DateTime.UtcNow;
                _unitOfWork.UserVectors.Update(vector);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserVector, object>(
                tableName: "UserVectors",
                recordId: vector.ProfileId,               // Primary key
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { EmbeddingJson = oldEmbedding },
                newValues: new { EmbeddingJson = vector.EmbeddingJson },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("User vector {Action} for ProfileId: {ProfileId}",
                isNew ? "created" : "updated", request.ProfileId);

            var result = await _unitOfWork.UserVectors.FindAsync(
                v => v.ProfileId == request.ProfileId,
                include: q => q.Include(v => v.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserVectorDto>(result.First());
        }
    }
}
