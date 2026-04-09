using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PersonAward
{
//    public class GetPersonAwardByIdQueryHandler : IRequestHandler<GetPersonAwardByIdQuery, BaseResponse<PersonAwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPersonAwardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonAwardDto>> Handle(GetPersonAwardByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PersonAwards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonAwardDto>.Fail("PersonAward not found");
//                
//                var dto = _mapper.Map<PersonAwardDto>(entity);
//                return BaseResponse<PersonAwardDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonAwardDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
