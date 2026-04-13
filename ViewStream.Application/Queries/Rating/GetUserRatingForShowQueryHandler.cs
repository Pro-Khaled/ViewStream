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
    public class GetUserRatingForShowQueryHandler : IRequestHandler<GetUserRatingForShowQuery, RatingDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserRatingForShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RatingDto?> Handle(GetUserRatingForShowQuery request, CancellationToken cancellationToken)
        {
            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == request.ProfileId && r.ShowId == request.ShowId,
                include: q => q.Include(r => r.Profile).Include(r => r.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var rating = ratings.FirstOrDefault();
            return rating == null ? null : _mapper.Map<RatingDto>(rating);
        }
    }
}
