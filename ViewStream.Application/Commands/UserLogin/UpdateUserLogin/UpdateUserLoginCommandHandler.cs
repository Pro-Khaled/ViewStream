using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLogin.UpdateUserLogin
{
//    public class UpdateUserLoginCommandHandler : IRequestHandler<UpdateUserLoginCommand, BaseResponse<UserLoginDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserLoginCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserLoginDto>> Handle(UpdateUserLoginCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserLogins.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserLoginDto>.Fail("UserLogin not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserLogins.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserLoginDto>(entity);
//                // return BaseResponse<UserLoginDto>.Ok(dto, "UserLogin updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserLoginDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
