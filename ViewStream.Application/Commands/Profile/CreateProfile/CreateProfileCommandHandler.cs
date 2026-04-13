using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.CreateProfile
{
    using Profile = ViewStream.Domain.Entities.Profile;
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = _mapper.Map<Profile>(request.Dto);
            profile.UserId = request.UserId;
            profile.CreatedAt = DateTime.UtcNow;
            profile.IsDeleted = false;

            await _unitOfWork.Profiles.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProfileDto>(profile);
        }
    }
}
