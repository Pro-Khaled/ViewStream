using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PersonalizedRow
{
//    public class GetPersonalizedRowByIdQueryHandler : IRequestHandler<GetPersonalizedRowByIdQuery, BaseResponse<PersonalizedRowDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPersonalizedRowByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonalizedRowDto>> Handle(GetPersonalizedRowByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PersonalizedRows.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonalizedRowDto>.Fail("PersonalizedRow not found");
//                
//                var dto = _mapper.Map<PersonalizedRowDto>(entity);
//                return BaseResponse<PersonalizedRowDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonalizedRowDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
