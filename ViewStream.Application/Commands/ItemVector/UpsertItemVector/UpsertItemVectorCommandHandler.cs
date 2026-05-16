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

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    public class UpsertItemVectorCommandHandler : IRequestHandler<UpsertItemVectorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertItemVectorCommandHandler> _logger;

        public UpsertItemVectorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpsertItemVectorCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpsertItemVectorCommand request, CancellationToken cancellationToken)
        {
            var vector = await _unitOfWork.ItemVectors.GetByIdAsync<long>(request.ShowId, cancellationToken);
            bool isNew = false;
            
            if (vector == null)
            {
                isNew = true;
                vector = new Domain.Entities.ItemVector
                {
                    ShowId = request.ShowId,
                    EmbeddingJson = request.Dto.EmbeddingJson,
                    LastUpdated = DateTime.UtcNow
                };
                await _unitOfWork.ItemVectors.AddAsync(vector, cancellationToken);
            }
            else
            {
                vector.EmbeddingJson = request.Dto.EmbeddingJson;
                vector.LastUpdated = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Domain.Entities.ItemVector, object>(
                tableName: "ItemVectors",
                recordId: vector.ShowId,
                action: isNew ? "CREATE" : "UPDATE",
                oldValues: isNew ? null : new { vector.EmbeddingJson, vector.LastUpdated },
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Item vector upserted for ShowId: {ShowId}", request.ShowId);
            return true;
        }
    }
}
