using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.UpdateSharedList
{
//    public class UpdateSharedListCommandHandler : IRequestHandler<UpdateSharedListCommand, BaseResponse<SharedListDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSharedListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SharedListDto>> Handle(UpdateSharedListCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SharedLists.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SharedListDto>.Fail("SharedList not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.SharedLists.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SharedListDto>(entity);
//                // return BaseResponse<SharedListDto>.Ok(dto, "SharedList updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SharedListDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
