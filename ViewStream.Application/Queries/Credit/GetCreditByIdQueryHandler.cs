using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Credit
{
//    public class GetCreditByIdQueryHandler : IRequestHandler<GetCreditByIdQuery, BaseResponse<CreditDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetCreditByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CreditDto>> Handle(GetCreditByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Credits.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CreditDto>.Fail("Credit not found");
//                
//                var dto = _mapper.Map<CreditDto>(entity);
//                return BaseResponse<CreditDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CreditDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
