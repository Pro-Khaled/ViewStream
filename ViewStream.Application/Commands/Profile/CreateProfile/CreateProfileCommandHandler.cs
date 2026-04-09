using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.CreateProfile
{
  //  public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, BaseResponse<ProfileDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ProfileDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Profile>(request);
  //              
  //              // await _unitOfWork.Profiles.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ProfileDto>(entity);
  //              // return BaseResponse<ProfileDto>.Ok(dto, "Profile created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ProfileDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
