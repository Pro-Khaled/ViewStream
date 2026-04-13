using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Profile
{
    public class GetActiveProfileQueryHandler : IRequestHandler<GetActiveProfileQuery, ProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActiveProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProfileDto?> Handle(GetActiveProfileQuery request, CancellationToken cancellationToken)
        {
            // In a real implementation, you might store the active profile ID in a claim or database.
            // For now, we just return the first non-deleted profile (or implement as needed).
            var profiles = await _unitOfWork.Profiles.FindAsync(
                p => p.UserId == request.UserId && p.IsDeleted != true,
                include: q => q.Include(p => p.User),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var profile = profiles.OrderBy(p => p.CreatedAt).FirstOrDefault();
            return profile == null ? null : _mapper.Map<ProfileDto>(profile);
        }
    }
}
