using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Friendship
{
    public class SearchFriendsQueryHandler : IRequestHandler<SearchFriendsQuery, PagedResult<FriendshipListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchFriendsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<FriendshipListItemDto>> Handle(SearchFriendsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Friendships.GetQueryable()
                .Where(f => f.UserId == request.UserId && f.Status == "accepted")
                .Include(f => f.Friend)
                .Where(f => f.Friend.FullName.Contains(request.SearchTerm) || f.Friend.Email.Contains(request.SearchTerm));

            var totalCount = await query.CountAsync(cancellationToken);
            var friendships = await query
                .OrderBy(f => f.Friend.FullName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<FriendshipListItemDto>
            {
                Items = _mapper.Map<List<FriendshipListItemDto>>(friendships),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
