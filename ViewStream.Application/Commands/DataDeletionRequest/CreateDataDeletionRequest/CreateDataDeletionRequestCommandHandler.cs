using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest
{
    using DataDeletionRequest = ViewStream.Domain.Entities.DataDeletionRequest;

    public class CreateDataDeletionRequestCommandHandler
        : IRequestHandler<CreateDataDeletionRequestCommand, DataDeletionRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateDataDeletionRequestCommandHandler> _logger;

        public CreateDataDeletionRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateDataDeletionRequestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<DataDeletionRequestDto> Handle(
            CreateDataDeletionRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserId: {UserId} requesting data deletion", request.UserId);

            // Check there is no pending request already
            var existing = await _unitOfWork.DataDeletionRequests.FindAsync(
                d => d.UserId == request.UserId && d.Status == "pending",
                asNoTracking: false,
                cancellationToken: cancellationToken);

            if (existing.Any())
            {
                _logger.LogWarning("UserId: {UserId} already has a pending data deletion request", request.UserId);
                // Return the existing one mapped with user navigation — load with include
                var existingWithUser = await _unitOfWork.DataDeletionRequests.FindAsync(
                    d => d.UserId == request.UserId && d.Status == "pending",
                    include: q => q.Include(d => d.User),
                    asNoTracking: true,
                    cancellationToken: cancellationToken);
                return _mapper.Map<DataDeletionRequestDto>(existingWithUser.First());
            }

            var deletionRequest = new DataDeletionRequest
            {
                UserId = request.UserId,
                Status = "pending",
                RequestedAt = DateTime.UtcNow
            };

            await _unitOfWork.DataDeletionRequests.AddAsync(deletionRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<DataDeletionRequest, object>(
                tableName: "DataDeletionRequests",
                recordId: deletionRequest.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { deletionRequest.UserId, deletionRequest.Status, deletionRequest.RequestedAt },
                changedByUserId: request.UserId);

            _logger.LogInformation("Data deletion request {RequestId} created for UserId: {UserId}",
                deletionRequest.Id, request.UserId);

            // Re-load with navigation to map UserEmail
            var reloaded = await _unitOfWork.DataDeletionRequests.FindAsync(
                d => d.Id == deletionRequest.Id,
                include: q => q.Include(d => d.User),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<DataDeletionRequestDto>(reloaded.First());
        }
    }
}
