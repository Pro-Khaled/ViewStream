using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserInteraction.UpdateUserInteraction
{
//    public class UpdateUserInteractionCommandHandler : IRequestHandler<UpdateUserInteractionCommand, BaseResponse<UserInteractionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserInteractionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserInteractionDto>> Handle(UpdateUserInteractionCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserInteractions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserInteractionDto>.Fail("UserInteraction not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserInteractions.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserInteractionDto>(entity);
//                // return BaseResponse<UserInteractionDto>.Ok(dto, "UserInteraction updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserInteractionDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
