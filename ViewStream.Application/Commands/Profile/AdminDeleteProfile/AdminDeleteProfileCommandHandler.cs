using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;
using ViewStream.Application.DTOs;
using System;
using ViewStream.Application.Helpers;

namespace ViewStream.Application.Commands.Profile.AdminDeleteProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;

    public class AdminDeleteProfileCommandHandler : IRequestHandler<AdminDeleteProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AdminDeleteProfileCommandHandler> _logger;

        public AdminDeleteProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AdminDeleteProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(AdminDeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            if (profile == null)
            {
                throw new NotFoundException("Profile", request.ProfileId);
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
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Profile soft-deleted by admin with Id: {ProfileId}", profile.Id);
            return true;
        }
    }
}

