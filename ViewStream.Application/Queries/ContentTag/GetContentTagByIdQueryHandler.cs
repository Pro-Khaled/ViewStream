using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentTag
{
//    public class GetContentTagByIdQueryHandler : IRequestHandler<GetContentTagByIdQuery, BaseResponse<ContentTagDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetContentTagByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ContentTagDto>> Handle(GetContentTagByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ContentTags.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ContentTagDto>.Fail("ContentTag not found");
//                
//                var dto = _mapper.Map<ContentTagDto>(entity);
//                return BaseResponse<ContentTagDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ContentTagDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
