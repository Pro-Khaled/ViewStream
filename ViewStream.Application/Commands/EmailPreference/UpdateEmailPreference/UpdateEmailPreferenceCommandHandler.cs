using AutoMapper;
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference
{
    using EmailPreference = ViewStream.Domain.Entities.EmailPreference;
    public class UpdateEmailPreferenceCommandHandler : IRequestHandler<UpdateEmailPreferenceCommand, EmailPreferenceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEmailPreferenceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EmailPreferenceDto> Handle(UpdateEmailPreferenceCommand request, CancellationToken cancellationToken)
        {
            var pref = await _unitOfWork.EmailPreferences.GetByIdAsync<long>(request.UserId, cancellationToken);

            if (pref == null)
            {
                pref = new EmailPreference { UserId = request.UserId };
                ApplyChanges(pref, request.Dto);
                await _unitOfWork.EmailPreferences.AddAsync(pref, cancellationToken);
            }
            else
            {
                ApplyChanges(pref, request.Dto);
                _unitOfWork.EmailPreferences.Update(pref);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EmailPreferenceDto>(pref);
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
