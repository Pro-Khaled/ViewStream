using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.UpdateGenre
{
//    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, BaseResponse<GenreDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<GenreDto>> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Genres.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<GenreDto>.Fail("Genre not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Genres.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<GenreDto>(entity);
//                // return BaseResponse<GenreDto>.Ok(dto, "Genre updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<GenreDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
