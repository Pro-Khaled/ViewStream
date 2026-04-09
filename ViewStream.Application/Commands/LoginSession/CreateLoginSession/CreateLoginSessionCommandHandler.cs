using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.CreateLoginSession
{
  //  public class CreateLoginSessionCommandHandler : IRequestHandler<CreateLoginSessionCommand, BaseResponse<LoginSessionDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateLoginSessionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<LoginSessionDto>> Handle(CreateLoginSessionCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<LoginSession>(request);
  //              
  //              // await _unitOfWork.LoginSessions.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<LoginSessionDto>(entity);
  //              // return BaseResponse<LoginSessionDto>.Ok(dto, "LoginSession created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<LoginSessionDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
