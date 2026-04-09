using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary
{
//    public class UpdateUserLibraryCommandHandler : IRequestHandler<UpdateUserLibraryCommand, BaseResponse<UserLibraryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserLibraryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserLibraryDto>> Handle(UpdateUserLibraryCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserLibrarys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserLibraryDto>.Fail("UserLibrary not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserLibrarys.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserLibraryDto>(entity);
//                // return BaseResponse<UserLibraryDto>.Ok(dto, "UserLibrary updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserLibraryDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
