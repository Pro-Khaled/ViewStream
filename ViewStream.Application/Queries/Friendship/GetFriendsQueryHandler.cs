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

namespace ViewStream.Application.Queries.Friendship
{
    public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, List<FriendshipListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFriendsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<FriendshipListItemDto>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Friendships.GetQueryable()
                .Where(f => f.UserId == request.UserId && f.Status == request.Status);

            var friendships = await query
                .Include(f => f.Friend)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<FriendshipListItemDto>>(friendships);
        }
    }
}
