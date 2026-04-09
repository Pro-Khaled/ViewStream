using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Notification
{
//    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, BaseResponse<NotificationDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetNotificationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<NotificationDto>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Notifications.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<NotificationDto>.Fail("Notification not found");
//                
//                var dto = _mapper.Map<NotificationDto>(entity);
//                return BaseResponse<NotificationDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<NotificationDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
