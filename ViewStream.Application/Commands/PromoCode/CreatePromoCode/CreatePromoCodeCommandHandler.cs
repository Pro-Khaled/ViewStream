using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.CreatePromoCode
{
  //  public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, BaseResponse<PromoCodeDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePromoCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PromoCodeDto>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PromoCode>(request);
  //              
  //              // await _unitOfWork.PromoCodes.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PromoCodeDto>(entity);
  //              // return BaseResponse<PromoCodeDto>.Ok(dto, "PromoCode created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PromoCodeDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
