using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Profile
{
    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProfileByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProfileDto?> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profiles = await _unitOfWork.Profiles.FindAsync(
                p => p.Id == request.Id && p.UserId == request.UserId && p.IsDeleted != true,
                include: q => q.Include(p => p.User),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var profile = profiles.FirstOrDefault();
            return profile == null ? null : _mapper.Map<ProfileDto>(profile);
        }
    }
}
