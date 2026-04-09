using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAward.CreateShowAward
{
  //  public class CreateShowAwardCommandHandler : IRequestHandler<CreateShowAwardCommand, BaseResponse<ShowAwardDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateShowAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ShowAwardDto>> Handle(CreateShowAwardCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ShowAward>(request);
  //              
  //              // await _unitOfWork.ShowAwards.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ShowAwardDto>(entity);
  //              // return BaseResponse<ShowAwardDto>.Ok(dto, "ShowAward created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ShowAwardDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
