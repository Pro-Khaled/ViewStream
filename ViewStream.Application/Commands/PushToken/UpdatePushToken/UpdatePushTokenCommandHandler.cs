using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PushToken.UpdatePushToken
{
//    public class UpdatePushTokenCommandHandler : IRequestHandler<UpdatePushTokenCommand, BaseResponse<PushTokenDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePushTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PushTokenDto>> Handle(UpdatePushTokenCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PushTokens.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PushTokenDto>.Fail("PushToken not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.PushTokens.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PushTokenDto>(entity);
//                // return BaseResponse<PushTokenDto>.Ok(dto, "PushToken updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PushTokenDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
