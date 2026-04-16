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

namespace ViewStream.Application.Queries.Notification
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Notifications.GetQueryable()
                .Where(n => n.UserId == request.UserId);

            if (request.UnreadOnly)
                query = query.Where(n => n.IsRead != true);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(request.Limit)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<NotificationDto>>(notifications);
        }
    }
}
