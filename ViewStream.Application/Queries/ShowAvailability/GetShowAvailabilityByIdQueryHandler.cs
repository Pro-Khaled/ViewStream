using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ShowAvailability
{
//    public class GetShowAvailabilityByIdQueryHandler : IRequestHandler<GetShowAvailabilityByIdQuery, BaseResponse<ShowAvailabilityDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetShowAvailabilityByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowAvailabilityDto>> Handle(GetShowAvailabilityByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ShowAvailabilitys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowAvailabilityDto>.Fail("ShowAvailability not found");
//                
//                var dto = _mapper.Map<ShowAvailabilityDto>(entity);
//                return BaseResponse<ShowAvailabilityDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowAvailabilityDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
