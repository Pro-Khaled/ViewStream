using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    using ItemVector = Domain.Entities.ItemVector;
    public class UpsertItemVectorCommandHandler : IRequestHandler<UpsertItemVectorCommand, ItemVectorDto>
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

        public async Task<ItemVectorDto> Handle(UpsertItemVectorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting item vector for ShowId: {ShowId}", request.ShowId);

            var vector = await _unitOfWork.ItemVectors.GetByIdAsync<long>(request.ShowId, cancellationToken);
            bool isNew = false;
            string? oldEmbedding = vector?.EmbeddingJson;

            if (vector == null)
            {
                isNew = true;
                vector = new ItemVector
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
                _unitOfWork.ItemVectors.Update(vector);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ItemVector, object>(
                tableName: "ItemVectors",
                recordId: vector.ShowId,
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { EmbeddingJson = oldEmbedding },
                newValues: new { vector.EmbeddingJson },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Item vector {Action} for ShowId: {ShowId}", isNew ? "created" : "updated", request.ShowId);

            var result = await _unitOfWork.ItemVectors.FindAsync(
                v => v.ShowId == request.ShowId,
                include: q => q.Include(v => v.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<ItemVectorDto>(result.First());
        }
    }
}
