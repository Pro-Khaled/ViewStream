using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ShowAvailability
{
//    public class GetAllShowAvailabilitysQueryHandler : IRequestHandler<GetAllShowAvailabilitysQuery, BaseResponse<PagedResult<ShowAvailabilityDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllShowAvailabilitysQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<ShowAvailabilityDto>>> Handle(GetAllShowAvailabilitysQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.ShowAvailabilitys.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<ShowAvailabilityDto>>(entityList);
//                var result = new PagedResult<ShowAvailabilityDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<ShowAvailabilityDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<ShowAvailabilityDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
