using AutoMapper;
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdatePaymentMethodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaymentMethodDto?> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var method = await _unitOfWork.PaymentMethods.GetByIdAsync<long>(request.Id, cancellationToken);
            if (method == null || method.UserId != request.UserId) return null;
            _mapper.Map(request.Dto, method);
            _unitOfWork.PaymentMethods.Update(method);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PaymentMethodDto>(method);
        }
    }
}
