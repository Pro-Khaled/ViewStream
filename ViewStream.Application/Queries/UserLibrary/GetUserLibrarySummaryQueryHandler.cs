using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserLibrary
{
    public class GetUserLibrarySummaryQueryHandler : IRequestHandler<GetUserLibrarySummaryQuery, UserLibrarySummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserLibrarySummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserLibrarySummaryDto> Handle(GetUserLibrarySummaryQuery request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.ProfileId == request.ProfileId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var itemList = items.ToList();
            return new UserLibrarySummaryDto
            {
                ProfileId = request.ProfileId,
                TotalItems = itemList.Count,
                CountByStatus = itemList
                    .GroupBy(ul => ul.Status)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}
