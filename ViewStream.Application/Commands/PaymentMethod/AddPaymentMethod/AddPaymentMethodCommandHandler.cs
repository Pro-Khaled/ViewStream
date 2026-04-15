using AutoMapper;
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod
{
    using PaymentMethod = ViewStream.Domain.Entities.PaymentMethod;
    public class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, PaymentMethodDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddPaymentMethodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaymentMethodDto> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (request.Dto.IsDefault == true)
            {
                var existing = await _unitOfWork.PaymentMethods.FindAsync(p => p.UserId == request.UserId, cancellationToken: cancellationToken);
                foreach (var pm in existing) pm.IsDefault = false;
            }
            var method = _mapper.Map<PaymentMethod>(request.Dto);
            method.UserId = request.UserId;
            method.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.PaymentMethods.AddAsync(method, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PaymentMethodDto>(method);
        }
    }
}
