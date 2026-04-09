using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ErrorLog.UpdateErrorLog
{
//    public class UpdateErrorLogCommandHandler : IRequestHandler<UpdateErrorLogCommand, BaseResponse<ErrorLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateErrorLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ErrorLogDto>> Handle(UpdateErrorLogCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ErrorLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ErrorLogDto>.Fail("ErrorLog not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ErrorLogs.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ErrorLogDto>(entity);
//                // return BaseResponse<ErrorLogDto>.Ok(dto, "ErrorLog updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ErrorLogDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
