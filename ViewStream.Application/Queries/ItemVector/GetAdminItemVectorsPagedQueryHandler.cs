using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ItemVector
{
    using ItemVector = ViewStream.Domain.Entities.ItemVector;
    /// <summary>
    /// Handles admin paged item-vector queries (filters/search/sorting/pagination).
    /// </summary>
    public class GetAdminItemVectorsPagedQueryHandler
        : IRequestHandler<GetAdminItemVectorsPagedQuery, PagedResult<AdminItemVectorListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAdminItemVectorsPagedQueryHandler> _logger;

        public GetAdminItemVectorsPagedQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAdminItemVectorsPagedQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PagedResult<AdminItemVectorListItemDto>> Handle(
            GetAdminItemVectorsPagedQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<ItemVector> query = _unitOfWork.ItemVectors.GetQueryable()
                    .AsNoTracking();

                // includeDeleted consistency: ItemVector entity has no IsDeleted column; ignore IncludeDeleted.
                if (request.ShowId.HasValue)
                    query = query.Where(v => v.ShowId == request.ShowId.Value);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim();
                    query = query.Where(v => v.Show != null &&
                                             v.Show.Title != null &&
                                             v.Show.Title.Contains(term));
                }

                if (request.UpdatedFrom.HasValue)
                    query = query.Where(v => v.LastUpdated >= request.UpdatedFrom.Value);
                if (request.UpdatedTo.HasValue)
                    query = query.Where(v => v.LastUpdated <= request.UpdatedTo.Value);

                // Sorting
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    switch (request.SortBy.Trim().ToLowerInvariant())
                    {
                        case "lastupdated":
                        case "last_updated":
                            query = request.SortDescending
                                ? query.OrderByDescending(v => v.LastUpdated)
                                : query.OrderBy(v => v.LastUpdated);
                            break;

                        case "showid":
                            query = request.SortDescending
                                ? query.OrderByDescending(v => v.ShowId)
                                : query.OrderBy(v => v.ShowId);
                            break;

                        default:
                            query = request.SortDescending
                                ? query.OrderByDescending(v => v.LastUpdated)
                                : query.OrderBy(v => v.LastUpdated);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(v => v.LastUpdated);
                }

                // Projection
                var projected = query.ProjectTo<AdminItemVectorListItemDto>(_mapper.ConfigurationProvider);

                var totalCount = await projected.CountAsync(cancellationToken);

                var items = await projected
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                return new PagedResult<AdminItemVectorListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get admin item-vectors paged result");
                throw;
            }
        }
    }
}
