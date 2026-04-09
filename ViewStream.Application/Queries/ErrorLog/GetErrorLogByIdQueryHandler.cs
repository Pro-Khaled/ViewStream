using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ErrorLog
{
//    public class GetErrorLogByIdQueryHandler : IRequestHandler<GetErrorLogByIdQuery, BaseResponse<ErrorLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetErrorLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ErrorLogDto>> Handle(GetErrorLogByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ErrorLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ErrorLogDto>.Fail("ErrorLog not found");
//                
//                var dto = _mapper.Map<ErrorLogDto>(entity);
//                return BaseResponse<ErrorLogDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ErrorLogDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
