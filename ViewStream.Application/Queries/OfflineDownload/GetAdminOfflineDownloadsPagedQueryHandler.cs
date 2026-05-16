using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.OfflineDownload
{
    public class GetAdminOfflineDownloadsPagedQueryHandler : IRequestHandler<GetAdminOfflineDownloadsPagedQuery, PagedResult<AdminOfflineDownloadListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminOfflineDownloadsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminOfflineDownloadListItemDto>> Handle(GetAdminOfflineDownloadsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.OfflineDownloads.GetQueryable()
                .AsNoTracking();

            if (request.ProfileId.HasValue)
                query = query.Where(od => od.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(od => od.Episode.Title.Contains(request.SearchTerm) || od.Profile.Name.Contains(request.SearchTerm));

            var projected = query.ProjectTo<AdminOfflineDownloadListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(od => od.DownloadedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminOfflineDownloadListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
