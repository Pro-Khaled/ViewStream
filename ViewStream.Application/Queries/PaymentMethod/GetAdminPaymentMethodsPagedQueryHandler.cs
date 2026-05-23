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

namespace ViewStream.Application.Queries.PaymentMethod
{
    public class GetAdminPaymentMethodsPagedQueryHandler : IRequestHandler<GetAdminPaymentMethodsPagedQuery, PagedResult<AdminPaymentMethodListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminPaymentMethodsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminPaymentMethodListItemDto>> Handle(GetAdminPaymentMethodsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PaymentMethods.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(pm => pm.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(pm => pm.Provider.Contains(request.SearchTerm) || pm.User.Email.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(pm => pm.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(pm => pm.CreatedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminPaymentMethodListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(pm => pm.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminPaymentMethodListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
