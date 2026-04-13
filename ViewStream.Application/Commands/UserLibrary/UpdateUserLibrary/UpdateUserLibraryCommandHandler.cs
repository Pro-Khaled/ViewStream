using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary
{
    public class UpdateUserLibraryCommandHandler : IRequestHandler<UpdateUserLibraryCommand, UserLibraryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateUserLibraryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserLibraryDto?> Handle(UpdateUserLibraryCommand request, CancellationToken cancellationToken)
        {
            var library = await _unitOfWork.UserLibraries.GetByIdAsync<long>(request.Id, cancellationToken);
            if (library == null || library.ProfileId != request.ProfileId)
                return null;

            var dto = request.Dto;
            if (dto.Status != null) library.Status = dto.Status;
            if (dto.EpisodesWatched.HasValue) library.EpisodesWatched = dto.EpisodesWatched;
            if (dto.UserScore.HasValue) library.UserScore = dto.UserScore;
            if (dto.StartedAt.HasValue) library.StartedAt = dto.StartedAt;
            if (dto.CompletedAt.HasValue) library.CompletedAt = dto.CompletedAt;
            library.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserLibraries.Update(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.Id == library.Id,
                include: q => q.Include(ul => ul.Profile)
                               .Include(ul => ul.Show)
                               .Include(ul => ul.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserLibraryDto>(result.First());
        }
    }
}
