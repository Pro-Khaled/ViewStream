using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.UpdateSharedListItem
{
//    public class UpdateSharedListItemCommandHandler : IRequestHandler<UpdateSharedListItemCommand, BaseResponse<SharedListItemDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSharedListItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SharedListItemDto>> Handle(UpdateSharedListItemCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SharedListItems.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SharedListItemDto>.Fail("SharedListItem not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.SharedListItems.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SharedListItemDto>(entity);
//                // return BaseResponse<SharedListItemDto>.Ok(dto, "SharedListItem updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SharedListItemDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
