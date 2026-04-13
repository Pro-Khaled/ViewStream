using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentTag
{
    public class GetContentTagsByCategoryQueryHandler : IRequestHandler<GetContentTagsByCategoryQuery, List<ContentTagListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetContentTagsByCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ContentTagListItemDto>> Handle(GetContentTagsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.ContentTags.FindAsync(
                predicate: t => t.Category == request.Category,
                include: q => q.Include(t => t.Shows),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<ContentTagListItemDto>>(tags.OrderBy(t => t.Name));
        }
    }
}
