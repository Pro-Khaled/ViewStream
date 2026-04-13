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
    public class GetSharedListByShareCodeQueryHandler : IRequestHandler<GetSharedListByShareCodeQuery, SharedListDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSharedListByShareCodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SharedListDto?> Handle(GetSharedListByShareCodeQuery request, CancellationToken cancellationToken)
        {
            var lists = await _unitOfWork.SharedLists.FindAsync(
                l => l.ShareCode == request.ShareCode && l.IsDeleted != true,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var list = lists.FirstOrDefault();
            return list == null ? null : _mapper.Map<SharedListDto>(list);
        }
    }
}
