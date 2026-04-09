using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Profile
{
//    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, BaseResponse<ProfileDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetProfileByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ProfileDto>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Profiles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ProfileDto>.Fail("Profile not found");
//                
//                var dto = _mapper.Map<ProfileDto>(entity);
//                return BaseResponse<ProfileDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ProfileDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
