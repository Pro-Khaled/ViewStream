using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Friendship
{
//    public class GetFriendshipByIdQueryHandler : IRequestHandler<GetFriendshipByIdQuery, BaseResponse<FriendshipDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetFriendshipByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<FriendshipDto>> Handle(GetFriendshipByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Friendships.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<FriendshipDto>.Fail("Friendship not found");
//                
//                var dto = _mapper.Map<FriendshipDto>(entity);
//                return BaseResponse<FriendshipDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<FriendshipDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
