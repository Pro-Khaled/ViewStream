using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ShowAward
{
//    public class GetShowAwardByIdQueryHandler : IRequestHandler<GetShowAwardByIdQuery, BaseResponse<ShowAwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetShowAwardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowAwardDto>> Handle(GetShowAwardByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ShowAwards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowAwardDto>.Fail("ShowAward not found");
//                
//                var dto = _mapper.Map<ShowAwardDto>(entity);
//                return BaseResponse<ShowAwardDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowAwardDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
