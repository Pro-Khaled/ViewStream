using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Season
{
//    public class GetSeasonByIdQueryHandler : IRequestHandler<GetSeasonByIdQuery, BaseResponse<SeasonDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSeasonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SeasonDto>> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Seasons.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SeasonDto>.Fail("Season not found");
//                
//                var dto = _mapper.Map<SeasonDto>(entity);
//                return BaseResponse<SeasonDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SeasonDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
