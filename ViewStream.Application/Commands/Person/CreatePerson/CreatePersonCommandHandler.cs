using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.CreatePerson
{
  //  public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, BaseResponse<PersonDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PersonDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Person>(request);
  //              
  //              // await _unitOfWork.Persons.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PersonDto>(entity);
  //              // return BaseResponse<PersonDto>.Ok(dto, "Person created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PersonDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
