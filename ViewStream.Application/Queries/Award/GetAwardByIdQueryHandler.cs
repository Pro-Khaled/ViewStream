using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Award
{
//    public class GetAwardByIdQueryHandler : IRequestHandler<GetAwardByIdQuery, BaseResponse<AwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAwardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AwardDto>> Handle(GetAwardByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Awards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AwardDto>.Fail("Award not found");
//                
//                var dto = _mapper.Map<AwardDto>(entity);
//                return BaseResponse<AwardDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AwardDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
