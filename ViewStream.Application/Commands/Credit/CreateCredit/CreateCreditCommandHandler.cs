using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.CreateCredit
{
  //  public class CreateCreditCommandHandler : IRequestHandler<CreateCreditCommand, BaseResponse<CreditDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateCreditCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<CreditDto>> Handle(CreateCreditCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Credit>(request);
  //              
  //              // await _unitOfWork.Credits.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<CreditDto>(entity);
  //              // return BaseResponse<CreditDto>.Ok(dto, "Credit created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<CreditDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
