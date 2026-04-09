using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.CreatePersonAward
{
  //  public class CreatePersonAwardCommandHandler : IRequestHandler<CreatePersonAwardCommand, BaseResponse<PersonAwardDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePersonAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PersonAwardDto>> Handle(CreatePersonAwardCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PersonAward>(request);
  //              
  //              // await _unitOfWork.PersonAwards.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PersonAwardDto>(entity);
  //              // return BaseResponse<PersonAwardDto>.Ok(dto, "PersonAward created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PersonAwardDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
