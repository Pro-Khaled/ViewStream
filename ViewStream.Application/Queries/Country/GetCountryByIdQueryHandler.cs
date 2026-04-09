using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Country
{
//    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, BaseResponse<CountryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetCountryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Countrys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CountryDto>.Fail("Country not found");
//                
//                var dto = _mapper.Map<CountryDto>(entity);
//                return BaseResponse<CountryDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CountryDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
