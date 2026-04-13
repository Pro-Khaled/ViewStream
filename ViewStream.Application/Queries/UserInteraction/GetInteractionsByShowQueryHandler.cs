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

namespace ViewStream.Application.Queries.UserInteraction
{
    public class GetInteractionsByShowQueryHandler : IRequestHandler<GetInteractionsByShowQuery, PagedResult<UserInteractionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetInteractionsByShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserInteractionListItemDto>> Handle(GetInteractionsByShowQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserInteractions.GetQueryable()
                .Where(i => i.ShowId == request.ShowId);

            var totalCount = await query.CountAsync(cancellationToken);

            var interactions = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(i => i.Profile)
                .Include(i => i.Show)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<UserInteractionListItemDto>
            {
                Items = _mapper.Map<List<UserInteractionListItemDto>>(interactions),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
