using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProfileDto?> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (profile == null || profile.UserId != request.UserId || profile.IsDeleted == true)
                return null;

            _mapper.Map(request.Dto, profile);
            profile.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Profiles.Update(profile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProfileDto>(profile);
        }
    }
}
