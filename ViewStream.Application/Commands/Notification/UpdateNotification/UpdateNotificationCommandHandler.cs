using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.UpdateNotification
{
//    public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, BaseResponse<NotificationDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateNotificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<NotificationDto>> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Notifications.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<NotificationDto>.Fail("Notification not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Notifications.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<NotificationDto>(entity);
//                // return BaseResponse<NotificationDto>.Ok(dto, "Notification updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<NotificationDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
