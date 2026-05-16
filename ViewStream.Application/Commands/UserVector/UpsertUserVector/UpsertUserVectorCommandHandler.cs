using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Application.Helpers;

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    using UserVector = Domain.Entities.UserVector;
    public class UpsertUserVectorCommandHandler : IRequestHandler<UpsertUserVectorCommand, bool>
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

        public async Task<bool> Handle(UpsertUserVectorCommand request, CancellationToken cancellationToken)
        {
            var vector = await _unitOfWork.UserVectors.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            bool isNew = false;
            
            if (vector == null)
            {
                isNew = true;
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
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<UserVector, object>(
                tableName: "UserVectors",
                recordId: vector.ProfileId,
                action: isNew ? "CREATE" : "UPDATE",
                oldValues: isNew ? null : new { vector.EmbeddingJson, vector.LastUpdated },
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("User vector upserted for ProfileId: {ProfileId}", request.ProfileId);
            return true;
        }
    }
}
