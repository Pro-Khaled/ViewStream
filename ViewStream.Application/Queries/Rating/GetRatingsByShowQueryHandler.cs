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

namespace ViewStream.Application.Queries.Rating
{
    public class GetRatingsByShowQueryHandler : IRequestHandler<GetRatingsByShowQuery, List<RatingListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRatingsByShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RatingListItemDto>> Handle(GetRatingsByShowQuery request, CancellationToken cancellationToken)
        {
            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.ShowId == request.ShowId,
                include: q => q.Include(r => r.Profile),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<RatingListItemDto>>(ratings.OrderByDescending(r => r.RatedAt));
        }
    }
}
