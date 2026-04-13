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

namespace ViewStream.Application.Queries.SharedListItem
{
    public class GetItemsBySharedListQueryHandler : IRequestHandler<GetItemsBySharedListQuery, List<SharedListItemListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetItemsBySharedListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SharedListItemListItemDto>> Handle(GetItemsBySharedListQuery request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == request.ListId,
                include: q => q.Include(i => i.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<SharedListItemListItemDto>>(items.OrderByDescending(i => i.AddedAt));
        }
    }
}
