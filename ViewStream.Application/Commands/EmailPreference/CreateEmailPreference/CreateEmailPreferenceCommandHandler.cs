using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EmailPreference.CreateEmailPreference
{
  //  public class CreateEmailPreferenceCommandHandler : IRequestHandler<CreateEmailPreferenceCommand, BaseResponse<EmailPreferenceDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateEmailPreferenceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<EmailPreferenceDto>> Handle(CreateEmailPreferenceCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<EmailPreference>(request);
  //              
  //              // await _unitOfWork.EmailPreferences.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<EmailPreferenceDto>(entity);
  //              // return BaseResponse<EmailPreferenceDto>.Ok(dto, "EmailPreference created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<EmailPreferenceDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
