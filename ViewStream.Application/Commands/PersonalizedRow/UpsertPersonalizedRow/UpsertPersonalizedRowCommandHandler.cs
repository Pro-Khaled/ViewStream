using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;
    public class UpsertPersonalizedRowCommandHandler : IRequestHandler<UpsertPersonalizedRowCommand, PersonalizedRowDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertPersonalizedRowCommandHandler> _logger;

        public UpsertPersonalizedRowCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<UpsertPersonalizedRowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PersonalizedRowDto> Handle(UpsertPersonalizedRowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting personalized row '{RowName}' for ProfileId: {ProfileId}",
                request.Dto.RowName, request.ProfileId);

            var existing = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId && r.RowName == request.Dto.RowName,
                cancellationToken: cancellationToken);

            var row = existing.FirstOrDefault();
            var showIdsJson = JsonSerializer.Serialize(request.Dto.ShowIds);
            bool isNew = row == null;
            string? oldShowIdsJson = row?.ShowIdsJson;

            if (isNew)
            {
                row = new PersonalizedRow
                {
                    ProfileId = request.ProfileId,
                    RowName = request.Dto.RowName,
                    ShowIdsJson = showIdsJson,
                    GeneratedAt = DateTime.UtcNow
                };
                await _unitOfWork.PersonalizedRows.AddAsync(row, cancellationToken);
            }
            else
            {
                row.ShowIdsJson = showIdsJson;
                row.GeneratedAt = DateTime.UtcNow;
                _unitOfWork.PersonalizedRows.Update(row);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonalizedRow, object>(
                tableName: "PersonalizedRows",
                recordId: row.ProfileId.GetHashCode() ^ row.RowName.GetHashCode(),
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { RowName = row.RowName, ShowIds = oldShowIdsJson },
                newValues: new { row.RowName, ShowIds = showIdsJson },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Personalized row '{RowName}' {Action} for ProfileId: {ProfileId}",
                row.RowName, isNew ? "created" : "updated", request.ProfileId);

            return new PersonalizedRowDto
            {
                ProfileId = row.ProfileId,
                RowName = row.RowName,
                ShowIds = request.Dto.ShowIds,
                GeneratedAt = row.GeneratedAt
            };
        }
    }
}
