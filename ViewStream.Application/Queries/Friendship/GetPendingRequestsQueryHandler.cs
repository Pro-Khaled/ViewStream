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
    using Friendship = Domain.Entities.Friendship;
    public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, List<FriendshipListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<FriendshipListItemDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Friendship> query;
            if (request.Direction == "sent")
            {
                query = _unitOfWork.Friendships.GetQueryable()
                    .Where(f => f.UserId == request.UserId && f.Status == "pending");
            }
            else // received
            {
                query = _unitOfWork.Friendships.GetQueryable()
                    .Where(f => f.FriendId == request.UserId && f.Status == "pending");
            }

            var friendships = await query
                .Include(f => request.Direction == "sent" ? f.Friend : f.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<FriendshipListItemDto>>(friendships);
        }
    }
}
