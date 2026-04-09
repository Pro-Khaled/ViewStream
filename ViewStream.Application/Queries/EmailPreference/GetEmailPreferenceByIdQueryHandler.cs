using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EmailPreference
{
//    public class GetEmailPreferenceByIdQueryHandler : IRequestHandler<GetEmailPreferenceByIdQuery, BaseResponse<EmailPreferenceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetEmailPreferenceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EmailPreferenceDto>> Handle(GetEmailPreferenceByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.EmailPreferences.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EmailPreferenceDto>.Fail("EmailPreference not found");
//                
//                var dto = _mapper.Map<EmailPreferenceDto>(entity);
//                return BaseResponse<EmailPreferenceDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EmailPreferenceDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
