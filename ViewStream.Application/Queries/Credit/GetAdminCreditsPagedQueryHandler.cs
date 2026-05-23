using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Credit
{
    public class GetAdminCreditsPagedQueryHandler
        : IRequestHandler<GetAdminCreditsPagedQuery, PagedResult<AdminCreditListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminCreditsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminCreditListItemDto>> Handle(
            GetAdminCreditsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Credits.GetQueryable();

            query = query.Include(e => e.Person);             query = query.Include(e => e.Show);             query = query.Include(e => e.Season);             query = query.Include(e => e.Episode);


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    x.Person.Name.Contains(term) ||
                    x.Role.Contains(term) ||
                    (x.Show != null && x.Show.Title.Contains(term)) ||
                    (x.Season != null && x.Season.Title.Contains(term)) ||
                    (x.Episode != null && x.Episode.Title.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
                query = query.Where(s => s.Role == request.Role);

            var projected = query.Select(s => new AdminCreditListItemDto
            {
                Id = s.Id,
                PersonId = s.PersonId,
                PersonName = s.Person.Name,
                PersonPhotoUrl = s.Person.PhotoUrl,
                Role = s.Role,
                CharacterName = s.CharacterName,
                ShowId = s.ShowId,
                SeasonId = s.SeasonId,
                EpisodeId = s.EpisodeId,
                TargetType = s.Show != null ? "Show" : s.Season != null ? "Season" : "Episode",
                TargetTitle = s.Show != null ? s.Show.Title : s.Season != null ? s.Season.Show.Title + " S" + s.Season.SeasonNumber : s.Episode.Title,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "targettype" => desc ? projected.OrderByDescending(x => x.TargetType) : projected.OrderBy(x => x.TargetType),
                    "targettitle" => desc ? projected.OrderByDescending(x => x.TargetTitle) : projected.OrderBy(x => x.TargetTitle),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.Id);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminCreditListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
