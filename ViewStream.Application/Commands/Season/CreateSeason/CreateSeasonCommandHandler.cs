using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.CreateSeason
{
  //  public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, BaseResponse<SeasonDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSeasonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SeasonDto>> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Season>(request);
  //              
  //              // await _unitOfWork.Seasons.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SeasonDto>(entity);
  //              // return BaseResponse<SeasonDto>.Ok(dto, "Season created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SeasonDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
