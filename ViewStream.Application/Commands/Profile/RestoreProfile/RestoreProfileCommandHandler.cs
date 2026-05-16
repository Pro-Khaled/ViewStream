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

namespace ViewStream.Application.Commands.Profile.RestoreProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;

    public class RestoreProfileCommandHandler : IRequestHandler<RestoreProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreProfileCommandHandler> _logger;

        public RestoreProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            if (profile == null)
            {
                throw new NotFoundException("Profile", request.ProfileId);
            }

            var oldValues = _mapper.Map<ProfileDto>(profile);
            profile.IsDeleted = false;
            profile.DeletedAt = null;
            profile.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Profile, object>(
                tableName: "Profiles",
                recordId: profile.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Profile restored with Id: {ProfileId}", profile.Id);
            return true;
        }
    }
}

