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

namespace ViewStream.Application.Queries.SharedList
{
    public class GetSharedListsByProfileQueryHandler : IRequestHandler<GetSharedListsByProfileQuery, List<SharedListListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSharedListsByProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SharedListListItemDto>> Handle(GetSharedListsByProfileQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SharedLists.GetQueryable()
                .Where(l => l.OwnerProfileId == request.ProfileId && l.IsDeleted != true);

            if (!request.IncludePrivate)
                query = query.Where(l => l.IsPublic == true);

            var lists = await query
                .OrderByDescending(l => l.CreatedAt)
                .Include(l => l.OwnerProfile)
                .Include(l => l.SharedListItems)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SharedListListItemDto>>(lists);
        }
    }
}
