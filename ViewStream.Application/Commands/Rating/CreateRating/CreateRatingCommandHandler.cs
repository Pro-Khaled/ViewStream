using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.CreateRating
{
  //  public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, BaseResponse<RatingDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateRatingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<RatingDto>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Rating>(request);
  //              
  //              // await _unitOfWork.Ratings.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<RatingDto>(entity);
  //              // return BaseResponse<RatingDto>.Ok(dto, "Rating created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<RatingDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
