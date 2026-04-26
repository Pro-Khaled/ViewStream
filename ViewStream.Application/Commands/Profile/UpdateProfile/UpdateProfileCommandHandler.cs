using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.UpdateProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ProfileDto?> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile Id: {ProfileId} for UserId: {UserId}",
                request.Id, request.UserId);

            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (profile == null || profile.UserId != request.UserId || profile.IsDeleted == true)
            {
                _logger.LogWarning("Profile not found or access denied. Id: {ProfileId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<ProfileDto>(profile);
            _mapper.Map(request.Dto, profile);
            profile.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Profiles.Update(profile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Profile, object>(
                tableName: "Profiles",
                recordId: profile.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile updated. Id: {ProfileId}", profile.Id);
            return _mapper.Map<ProfileDto>(profile);
        }
    }
}
