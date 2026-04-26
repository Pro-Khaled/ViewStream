using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.CreateProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateProfileCommandHandler> _logger;

        public CreateProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating profile for UserId: {UserId}, Name: {ProfileName}",
                request.UserId, request.Dto.Name);

            var profile = _mapper.Map<Profile>(request.Dto);
            profile.UserId = request.UserId;
            profile.CreatedAt = DateTime.UtcNow;
            profile.IsDeleted = false;

            await _unitOfWork.Profiles.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Profile, object>(
                tableName: "Profiles",
                recordId: profile.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile created with Id: {ProfileId}", profile.Id);
            return _mapper.Map<ProfileDto>(profile);
        }
    }
}
