using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Profile
{
    public class GetProfilesByUserQueryHandler : IRequestHandler<GetProfilesByUserQuery, List<ProfileListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProfilesByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProfileListItemDto>> Handle(GetProfilesByUserQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Profiles.GetQueryable()
                .Where(p => p.UserId == request.UserId);

            if (!request.IncludeDeleted)
                query = query.Where(p => p.IsDeleted != true);

            var profiles = await query
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ProfileListItemDto>>(profiles);
        }
    }
}
