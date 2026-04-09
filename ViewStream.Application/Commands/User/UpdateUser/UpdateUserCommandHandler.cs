using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.User.UpdateUser
{
//    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse<UserDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Users.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserDto>.Fail("User not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Users.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserDto>(entity);
//                // return BaseResponse<UserDto>.Ok(dto, "User updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
