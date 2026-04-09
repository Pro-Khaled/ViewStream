using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.CreateShow
{
  //  public class CreateShowCommandHandler : IRequestHandler<CreateShowCommand, BaseResponse<ShowDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateShowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ShowDto>> Handle(CreateShowCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Show>(request);
  //              
  //              // await _unitOfWork.Shows.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ShowDto>(entity);
  //              // return BaseResponse<ShowDto>.Ok(dto, "Show created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ShowDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
