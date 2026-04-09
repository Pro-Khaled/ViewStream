using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserToken.UpdateUserToken
{
//    public class UpdateUserTokenCommandHandler : IRequestHandler<UpdateUserTokenCommand, BaseResponse<UserTokenDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserTokenDto>> Handle(UpdateUserTokenCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserTokens.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserTokenDto>.Fail("UserToken not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserTokens.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserTokenDto>(entity);
//                // return BaseResponse<UserTokenDto>.Ok(dto, "UserToken updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserTokenDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
