using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest
{
    using DataDeletionRequest = ViewStream.Domain.Entities.DataDeletionRequest;
    public class UpdateDataDeletionRequestCommandHandler : IRequestHandler<UpdateDataDeletionRequestCommand, DataDeletionRequestDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateDataDeletionRequestCommandHandler> _logger;

        public UpdateDataDeletionRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateDataDeletionRequestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<DataDeletionRequestDto?> Handle(UpdateDataDeletionRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating DataDeletionRequest {Id} to status {Status}", request.Id, request.Dto.Status);

            var req = await _unitOfWork.DataDeletionRequests.GetByIdAsync<long>(request.Id, cancellationToken);
            if (req == null)
                return null;

            var oldValues = _mapper.Map<DataDeletionRequestDto>(req);
            if (request.Dto.Status != null) req.Status = request.Dto.Status;
            if (request.Dto.ConfirmationCode != null) req.ConfirmationCode = request.Dto.ConfirmationCode;
            if (request.Dto.Status == "completed") req.CompletedAt = DateTime.UtcNow;

            _unitOfWork.DataDeletionRequests.Update(req);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<DataDeletionRequest, object>(
                tableName: "DataDeletionRequests",
                recordId: req.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: _mapper.Map<DataDeletionRequestDto>(req),
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("DataDeletionRequest {Id} updated", req.Id);
            return _mapper.Map<DataDeletionRequestDto>(req);
        }
    }
}
