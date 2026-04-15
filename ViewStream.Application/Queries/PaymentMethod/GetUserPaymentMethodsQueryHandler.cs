using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PaymentMethod
{
    public class GetUserPaymentMethodsQueryHandler : IRequestHandler<GetUserPaymentMethodsQuery, List<PaymentMethodDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetUserPaymentMethodsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<PaymentMethodDto>> Handle(GetUserPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var methods = await _unitOfWork.PaymentMethods.FindAsync(
                p => p.UserId == request.UserId,
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<PaymentMethodDto>>(methods.OrderByDescending(p => p.IsDefault).ThenByDescending(p => p.CreatedAt));
        }
    }
}
