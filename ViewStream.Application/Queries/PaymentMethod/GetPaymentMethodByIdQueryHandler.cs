using AutoMapper;
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PaymentMethod
{
    public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethodDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPaymentMethodByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaymentMethodDto?> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
        {
            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null || method.UserId != request.UserId) return null;
            return _mapper.Map<PaymentMethodDto>(method);
        }
    }
}
