using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Rating
{
    public class GetShowRatingSummaryQueryHandler : IRequestHandler<GetShowRatingSummaryQuery, ShowRatingSummaryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetShowRatingSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShowRatingSummaryDto?> Handle(GetShowRatingSummaryQuery request, CancellationToken cancellationToken)
        {
            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null || show.IsDeleted == true) return null;

            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.ShowId == request.ShowId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var ratingList = ratings.ToList();
            var average = ratingList.Any() ? ratingList.Average(r => r.Rating1) : 0;

            return new ShowRatingSummaryDto
            {
                ShowId = request.ShowId,
                ShowTitle = show.Title,
                AverageRating = Math.Round(average, 1),
                TotalRatings = ratingList.Count
            };
        }
    }
}
