using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EmailPreference
{
//    public class GetAllEmailPreferencesQueryHandler : IRequestHandler<GetAllEmailPreferencesQuery, BaseResponse<PagedResult<EmailPreferenceDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllEmailPreferencesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<EmailPreferenceDto>>> Handle(GetAllEmailPreferencesQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.EmailPreferences.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<EmailPreferenceDto>>(entityList);
//                var result = new PagedResult<EmailPreferenceDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<EmailPreferenceDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<EmailPreferenceDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}
