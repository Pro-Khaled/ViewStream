using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.UpdateFriendship
{
//    public class UpdateFriendshipCommandHandler : IRequestHandler<UpdateFriendshipCommand, BaseResponse<FriendshipDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateFriendshipCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<FriendshipDto>> Handle(UpdateFriendshipCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Friendships.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<FriendshipDto>.Fail("Friendship not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Friendships.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<FriendshipDto>(entity);
//                // return BaseResponse<FriendshipDto>.Ok(dto, "Friendship updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<FriendshipDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
