using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Notification
{
//    public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, BaseResponse<PagedResult<NotificationDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllNotificationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<NotificationDto>>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.Notifications.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<NotificationDto>>(entityList);
//                var result = new PagedResult<NotificationDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<NotificationDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<NotificationDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
