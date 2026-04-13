using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SharedList
{
    public class GetSharedListByIdQueryHandler : IRequestHandler<GetSharedListByIdQuery, SharedListDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSharedListByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SharedListDto?> Handle(GetSharedListByIdQuery request, CancellationToken cancellationToken)
        {
            var lists = await _unitOfWork.SharedLists.FindAsync(
                l => l.Id == request.Id && l.IsDeleted != true,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var list = lists.FirstOrDefault();
            if (list == null) return null;

            // Permission check: owner or public or requesting profile matches owner
            if (list.IsPublic == false && list.OwnerProfileId != request.RequestingProfileId)
                return null;

            return _mapper.Map<SharedListDto>(list);
        }
    }
}
