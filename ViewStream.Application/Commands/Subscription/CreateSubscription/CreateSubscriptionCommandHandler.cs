using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
  //  public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, BaseResponse<SubscriptionDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SubscriptionDto>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Subscription>(request);
  //              
  //              // await _unitOfWork.Subscriptions.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SubscriptionDto>(entity);
  //              // return BaseResponse<SubscriptionDto>.Ok(dto, "Subscription created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SubscriptionDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
