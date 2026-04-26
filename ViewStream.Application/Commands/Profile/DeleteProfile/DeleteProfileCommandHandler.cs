using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.DeleteProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteProfileCommandHandler> _logger;

        public DeleteProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting profile Id: {ProfileId} for UserId: {UserId}",
                request.Id, request.UserId);

            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (profile == null || profile.UserId != request.UserId || profile.IsDeleted == true)
            {
                _logger.LogWarning("Profile not found or already deleted. Id: {ProfileId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ProfileDto>(profile);
            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Profile, object>(
                tableName: "Profiles",
                recordId: profile.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile soft-deleted. Id: {ProfileId}", request.Id);
            return true;
        }
    }
}
