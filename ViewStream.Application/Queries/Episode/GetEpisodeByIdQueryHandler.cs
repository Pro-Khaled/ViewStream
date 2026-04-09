using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode
{
//    public class GetEpisodeByIdQueryHandler : IRequestHandler<GetEpisodeByIdQuery, BaseResponse<EpisodeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetEpisodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EpisodeDto>> Handle(GetEpisodeByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Episodes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EpisodeDto>.Fail("Episode not found");
//                
//                var dto = _mapper.Map<EpisodeDto>(entity);
//                return BaseResponse<EpisodeDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EpisodeDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
