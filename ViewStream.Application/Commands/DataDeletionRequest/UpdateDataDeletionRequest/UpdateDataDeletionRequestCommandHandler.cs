using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest
{
//    public class UpdateDataDeletionRequestCommandHandler : IRequestHandler<UpdateDataDeletionRequestCommand, BaseResponse<DataDeletionRequestDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateDataDeletionRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<DataDeletionRequestDto>> Handle(UpdateDataDeletionRequestCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.DataDeletionRequests.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<DataDeletionRequestDto>.Fail("DataDeletionRequest not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.DataDeletionRequests.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<DataDeletionRequestDto>(entity);
//                // return BaseResponse<DataDeletionRequestDto>.Ok(dto, "DataDeletionRequest updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<DataDeletionRequestDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
