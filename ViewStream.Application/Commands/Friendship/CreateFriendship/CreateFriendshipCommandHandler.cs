using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.CreateFriendship
{
  //  public class CreateFriendshipCommandHandler : IRequestHandler<CreateFriendshipCommand, BaseResponse<FriendshipDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateFriendshipCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<FriendshipDto>> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Friendship>(request);
  //              
  //              // await _unitOfWork.Friendships.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<FriendshipDto>(entity);
  //              // return BaseResponse<FriendshipDto>.Ok(dto, "Friendship created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<FriendshipDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
