using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PersonalizedRow
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;
    /// <summary>
    /// Handles admin paged personalized row queries.
    /// </summary>
    public class GetAdminPersonalizedRowsPagedQueryHandler
        : IRequestHandler<GetAdminPersonalizedRowsPagedQuery, PagedResult<AdminPersonalizedRowListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAdminPersonalizedRowsPagedQueryHandler> _logger;

        public GetAdminPersonalizedRowsPagedQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAdminPersonalizedRowsPagedQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PagedResult<AdminPersonalizedRowListItemDto>> Handle(
            GetAdminPersonalizedRowsPagedQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<PersonalizedRow> query = _unitOfWork.PersonalizedRows
                    .GetQueryable()
                    .AsNoTracking()
                    .Include(r => r.Profile);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim();
                    query = query.Where(r =>
                        r.Profile != null &&
                        r.Profile.Name != null &&
                        r.Profile.Name.Contains(term));
                }

                // Sorting:
                // Supported sortBy values:
                // - generatedat
                // - rowname
                // Any other/empty -> generatedat desc
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var sortBy = request.SortBy.Trim().ToLowerInvariant();
                    query = sortBy switch
                    {
                        "generatedat" => request.SortDescending
                            ? query.OrderByDescending(r => r.GeneratedAt)
                            : query.OrderBy(r => r.GeneratedAt),

                        "rowname" => request.SortDescending
                            ? query.OrderByDescending(r => r.RowName)
                            : query.OrderBy(r => r.RowName),

                        _ => request.SortDescending
                            ? query.OrderByDescending(r => r.GeneratedAt)
                            : query.OrderBy(r => r.GeneratedAt)
                    };
                }
                else
                {
                    query = query.OrderByDescending(r => r.GeneratedAt);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var pageRows = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(r => new
                    {
                        r.ProfileId,
                        r.RowName,
                        r.GeneratedAt,
                        r.ShowIdsJson
                    })
                    .ToListAsync(cancellationToken);

                var items = pageRows.Select(r =>
                {
                    int itemCount = 0;

                    if (!string.IsNullOrWhiteSpace(r.ShowIdsJson))
                    {
                        try
                        {
                            var ids = JsonSerializer.Deserialize<List<long>>(r.ShowIdsJson);
                            itemCount = ids?.Count ?? 0;
                        }
                        catch (JsonException)
                        {
                            _logger.LogWarning(
                                "Failed to parse ShowIdsJson for PersonalizedRow ProfileId={ProfileId}, RowName={RowName}",
                                r.ProfileId,
                                r.RowName);
                            itemCount = 0;
                        }
                    }

                    return new AdminPersonalizedRowListItemDto
                    {
                        ProfileId = r.ProfileId,
                        RowName = r.RowName,
                        GeneratedAt = r.GeneratedAt,
                        ItemCount = itemCount
                    };
                }).ToList();

                return new PagedResult<AdminPersonalizedRowListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get admin personalized rows paged result");
                throw;
            }
        }
    }
}
