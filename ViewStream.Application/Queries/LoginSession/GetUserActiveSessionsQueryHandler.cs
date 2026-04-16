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

namespace ViewStream.Application.Queries.LoginSession
{
    public class GetUserActiveSessionsQueryHandler : IRequestHandler<GetUserActiveSessionsQuery, List<LoginSessionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserActiveSessionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<LoginSessionListItemDto>> Handle(GetUserActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _unitOfWork.LoginSessions.FindAsync(
                s => s.UserId == request.UserId && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow,
                include: q => q.Include(s => s.Device),
                asNoTracking: true, cancellationToken: cancellationToken);

            return _mapper.Map<List<LoginSessionListItemDto>>(sessions.OrderByDescending(s => s.CreatedAt));
        }
    }
}
