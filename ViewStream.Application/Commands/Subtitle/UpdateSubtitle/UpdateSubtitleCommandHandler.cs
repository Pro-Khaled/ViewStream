using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.UpdateSubtitle
{
//    public class UpdateSubtitleCommandHandler : IRequestHandler<UpdateSubtitleCommand, BaseResponse<SubtitleDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSubtitleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SubtitleDto>> Handle(UpdateSubtitleCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Subtitles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SubtitleDto>.Fail("Subtitle not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Subtitles.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SubtitleDto>(entity);
//                // return BaseResponse<SubtitleDto>.Ok(dto, "Subtitle updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SubtitleDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
