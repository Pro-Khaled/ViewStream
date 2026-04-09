using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.CreateGenre
{
  //  public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, BaseResponse<GenreDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Genre>(request);
  //              
  //              // await _unitOfWork.Genres.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<GenreDto>(entity);
  //              // return BaseResponse<GenreDto>.Ok(dto, "Genre created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<GenreDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
