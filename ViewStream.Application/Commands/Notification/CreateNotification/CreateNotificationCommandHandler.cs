using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.CreateNotification
{
  //  public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, BaseResponse<NotificationDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateNotificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<NotificationDto>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Notification>(request);
  //              
  //              // await _unitOfWork.Notifications.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<NotificationDto>(entity);
  //              // return BaseResponse<NotificationDto>.Ok(dto, "Notification created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<NotificationDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
