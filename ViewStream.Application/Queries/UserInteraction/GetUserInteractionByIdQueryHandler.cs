using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserInteraction
{
//    public class GetUserInteractionByIdQueryHandler : IRequestHandler<GetUserInteractionByIdQuery, BaseResponse<UserInteractionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserInteractionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserInteractionDto>> Handle(GetUserInteractionByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserInteractions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserInteractionDto>.Fail("UserInteraction not found");
//                
//                var dto = _mapper.Map<UserInteractionDto>(entity);
//                return BaseResponse<UserInteractionDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserInteractionDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
