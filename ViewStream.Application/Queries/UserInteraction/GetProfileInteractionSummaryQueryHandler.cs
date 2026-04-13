using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserInteraction
{
    public class GetProfileInteractionSummaryQueryHandler : IRequestHandler<GetProfileInteractionSummaryQuery, UserInteractionSummaryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileInteractionSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserInteractionSummaryDto?> Handle(GetProfileInteractionSummaryQuery request, CancellationToken cancellationToken)
        {
            var profileExists = await _unitOfWork.Profiles.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            if (profileExists == null) return null;

            var interactions = await _unitOfWork.UserInteractions.FindAsync(
                i => i.ProfileId == request.ProfileId,
                include: q => q.Include(i => i.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var interactionList = interactions.ToList();
            var summary = new UserInteractionSummaryDto
            {
                ProfileId = request.ProfileId,
                TotalInteractions = interactionList.Count,
                InteractionsByType = interactionList
                    .GroupBy(i => i.InteractionType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TopShows = interactionList
                    .GroupBy(i => new { i.ShowId, i.Show.Title })
                    .Select(g => new UserInteractionSummaryDto.TopShowDto
                    {
                        ShowId = g.Key.ShowId,
                        ShowTitle = g.Key.Title,
                        InteractionCount = g.Count()
                    })
                    .OrderByDescending(s => s.InteractionCount)
                    .Take(10)
                    .ToList()
            };

            return summary;
        }
    }
}
