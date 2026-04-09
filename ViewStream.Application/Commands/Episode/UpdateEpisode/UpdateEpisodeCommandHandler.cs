using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.UpdateEpisode
{
//    public class UpdateEpisodeCommandHandler : IRequestHandler<UpdateEpisodeCommand, BaseResponse<EpisodeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateEpisodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EpisodeDto>> Handle(UpdateEpisodeCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Episodes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EpisodeDto>.Fail("Episode not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Episodes.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<EpisodeDto>(entity);
//                // return BaseResponse<EpisodeDto>.Ok(dto, "Episode updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EpisodeDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
