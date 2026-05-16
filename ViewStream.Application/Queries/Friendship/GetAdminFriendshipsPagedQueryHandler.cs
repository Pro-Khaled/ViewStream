using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Friendship
{
    public class GetAdminFriendshipsPagedQueryHandler : IRequestHandler<GetAdminFriendshipsPagedQuery, PagedResult<AdminFriendshipListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminFriendshipsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminFriendshipListItemDto>> Handle(GetAdminFriendshipsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Friendships.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(f => f.UserId == request.UserId.Value || f.FriendId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(f => f.Status == request.Status);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(f => f.User.FullName.Contains(request.SearchTerm) || f.Friend.FullName.Contains(request.SearchTerm));

            var projected = query.ProjectTo<AdminFriendshipListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(f => f.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminFriendshipListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
