using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.UpdateContentTag
{
//    public class UpdateContentTagCommandHandler : IRequestHandler<UpdateContentTagCommand, BaseResponse<ContentTagDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateContentTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ContentTagDto>> Handle(UpdateContentTagCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ContentTags.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ContentTagDto>.Fail("ContentTag not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ContentTags.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ContentTagDto>(entity);
//                // return BaseResponse<ContentTagDto>.Ok(dto, "ContentTag updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ContentTagDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
