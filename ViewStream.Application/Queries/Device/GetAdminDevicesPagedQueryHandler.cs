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

namespace ViewStream.Application.Queries.Device
{
    public class GetAdminDevicesPagedQueryHandler : IRequestHandler<GetAdminDevicesPagedQuery, PagedResult<AdminDeviceListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminDevicesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminDeviceListItemDto>> Handle(GetAdminDevicesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Devices.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(d => d.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(d => (d.DeviceName != null && d.DeviceName.Contains(request.SearchTerm)) || d.DeviceId.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(d => d.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(d => d.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(d => d.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(d => d.UpdatedAt <= request.UpdatedTo.Value);

            var projected = query.ProjectTo<AdminDeviceListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(d => d.LastActive);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminDeviceListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
