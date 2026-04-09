using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack
{
//    public class UpdateAudioTrackCommandHandler : IRequestHandler<UpdateAudioTrackCommand, BaseResponse<AudioTrackDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateAudioTrackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AudioTrackDto>> Handle(UpdateAudioTrackCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.AudioTracks.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AudioTrackDto>.Fail("AudioTrack not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.AudioTracks.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<AudioTrackDto>(entity);
//                // return BaseResponse<AudioTrackDto>.Ok(dto, "AudioTrack updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AudioTrackDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
