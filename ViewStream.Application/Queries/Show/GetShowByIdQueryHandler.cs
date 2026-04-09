using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Show
{
//    public class GetShowByIdQueryHandler : IRequestHandler<GetShowByIdQuery, BaseResponse<ShowDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetShowByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowDto>> Handle(GetShowByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Shows.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowDto>.Fail("Show not found");
//                
//                var dto = _mapper.Map<ShowDto>(entity);
//                return BaseResponse<ShowDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
