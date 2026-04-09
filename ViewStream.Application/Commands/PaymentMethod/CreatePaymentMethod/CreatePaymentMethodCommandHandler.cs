using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod
{
  //  public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand, BaseResponse<PaymentMethodDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePaymentMethodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PaymentMethodDto>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PaymentMethod>(request);
  //              
  //              // await _unitOfWork.PaymentMethods.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PaymentMethodDto>(entity);
  //              // return BaseResponse<PaymentMethodDto>.Ok(dto, "PaymentMethod created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PaymentMethodDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
