using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserLibrary
{
//    public class GetUserLibraryByIdQueryHandler : IRequestHandler<GetUserLibraryByIdQuery, BaseResponse<UserLibraryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserLibraryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserLibraryDto>> Handle(GetUserLibraryByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserLibrarys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserLibraryDto>.Fail("UserLibrary not found");
//                
//                var dto = _mapper.Map<UserLibraryDto>(entity);
//                return BaseResponse<UserLibraryDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserLibraryDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
