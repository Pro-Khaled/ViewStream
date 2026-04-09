using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ItemVector.UpdateItemVector
{
//    public class UpdateItemVectorCommandHandler : IRequestHandler<UpdateItemVectorCommand, BaseResponse<ItemVectorDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateItemVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ItemVectorDto>> Handle(UpdateItemVectorCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ItemVectors.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ItemVectorDto>.Fail("ItemVector not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ItemVectors.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ItemVectorDto>(entity);
//                // return BaseResponse<ItemVectorDto>.Ok(dto, "ItemVector updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ItemVectorDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
