using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SearchLog.UpdateSearchLog
{
//    public class UpdateSearchLogCommandHandler : IRequestHandler<UpdateSearchLogCommand, BaseResponse<SearchLogDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSearchLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SearchLogDto>> Handle(UpdateSearchLogCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.SearchLogs.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SearchLogDto>.Fail("SearchLog not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.SearchLogs.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SearchLogDto>(entity);
//                // return BaseResponse<SearchLogDto>.Ok(dto, "SearchLog updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SearchLogDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
