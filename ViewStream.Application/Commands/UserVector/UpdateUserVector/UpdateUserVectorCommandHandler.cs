using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserVector.UpdateUserVector
{
//    public class UpdateUserVectorCommandHandler : IRequestHandler<UpdateUserVectorCommand, BaseResponse<UserVectorDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserVectorDto>> Handle(UpdateUserVectorCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserVectors.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserVectorDto>.Fail("UserVector not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserVectors.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserVectorDto>(entity);
//                // return BaseResponse<UserVectorDto>.Ok(dto, "UserVector updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserVectorDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
