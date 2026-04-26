using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference
{
    using EmailPreference = ViewStream.Domain.Entities.EmailPreference;
    public class UpdateEmailPreferenceCommandHandler : IRequestHandler<UpdateEmailPreferenceCommand, EmailPreferenceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateEmailPreferenceCommandHandler> _logger;

        public UpdateEmailPreferenceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateEmailPreferenceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<EmailPreferenceDto> Handle(UpdateEmailPreferenceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating email preferences for UserId: {UserId}", request.UserId);

            var pref = await _unitOfWork.EmailPreferences.GetByIdAsync<long>(request.UserId, cancellationToken);
            bool isNew = false;
            EmailPreferenceDto? oldValues = null;

            if (pref == null)
            {
                isNew = true;
                pref = new EmailPreference { UserId = request.UserId };
                ApplyChanges(pref, request.Dto);
                await _unitOfWork.EmailPreferences.AddAsync(pref, cancellationToken);
            }
            else
            {
                oldValues = _mapper.Map<EmailPreferenceDto>(pref);
                ApplyChanges(pref, request.Dto);
                _unitOfWork.EmailPreferences.Update(pref);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var newValues = _mapper.Map<EmailPreferenceDto>(pref);

            _auditContext.SetAudit<EmailPreference, object>(
                tableName: "EmailPreferences",
                recordId: pref.UserId,
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: oldValues,
                newValues: newValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Email preferences {Action} for UserId: {UserId}",
                isNew ? "created" : "updated", request.UserId);

            return newValues;
        }

        private static void ApplyChanges(EmailPreference pref, UpdateEmailPreferenceDto dto)
        {
            if (dto.MarketingEmails.HasValue) pref.MarketingEmails = dto.MarketingEmails.Value;
            if (dto.NewReleaseAlerts.HasValue) pref.NewReleaseAlerts = dto.NewReleaseAlerts.Value;
            if (dto.RecommendationEmails.HasValue) pref.RecommendationEmails = dto.RecommendationEmails.Value;
            if (dto.AccountUpdates.HasValue) pref.AccountUpdates = dto.AccountUpdates.Value;
        }
    }
}
