using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.CreateCountry
{
  //  public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, BaseResponse<CountryDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateCountryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Country>(request);
  //              
  //              // await _unitOfWork.Countrys.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<CountryDto>(entity);
  //              // return BaseResponse<CountryDto>.Ok(dto, "Country created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<CountryDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
