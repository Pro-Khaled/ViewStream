using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.CreatePersonalizedRow
{
  //  public class CreatePersonalizedRowCommandHandler : IRequestHandler<CreatePersonalizedRowCommand, BaseResponse<PersonalizedRowDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePersonalizedRowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PersonalizedRowDto>> Handle(CreatePersonalizedRowCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PersonalizedRow>(request);
  //              
  //              // await _unitOfWork.PersonalizedRows.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PersonalizedRowDto>(entity);
  //              // return BaseResponse<PersonalizedRowDto>.Ok(dto, "PersonalizedRow created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PersonalizedRowDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
