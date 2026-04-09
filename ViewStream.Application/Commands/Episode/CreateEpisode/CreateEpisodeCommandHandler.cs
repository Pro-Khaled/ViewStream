using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.CreateEpisode
{
  //  public class CreateEpisodeCommandHandler : IRequestHandler<CreateEpisodeCommand, BaseResponse<EpisodeDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateEpisodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<EpisodeDto>> Handle(CreateEpisodeCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Episode>(request);
  //              
  //              // await _unitOfWork.Episodes.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<EpisodeDto>(entity);
  //              // return BaseResponse<EpisodeDto>.Ok(dto, "Episode created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<EpisodeDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
