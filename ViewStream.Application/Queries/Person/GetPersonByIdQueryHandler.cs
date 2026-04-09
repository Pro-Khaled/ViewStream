using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Person
{
//    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, BaseResponse<PersonDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPersonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonDto>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Persons.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonDto>.Fail("Person not found");
//                
//                var dto = _mapper.Map<PersonDto>(entity);
//                return BaseResponse<PersonDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
