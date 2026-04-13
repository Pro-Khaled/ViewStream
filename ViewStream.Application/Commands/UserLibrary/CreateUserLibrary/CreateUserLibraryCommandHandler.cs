using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.CreateUserLibrary
{
    using UserLibrary = ViewStream.Domain.Entities.UserLibrary;
    public class CreateUserLibraryCommandHandler : IRequestHandler<CreateUserLibraryCommand, UserLibraryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateUserLibraryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserLibraryDto> Handle(CreateUserLibraryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!dto.ShowId.HasValue && !dto.SeasonId.HasValue)
                throw new ArgumentException("Either ShowId or SeasonId must be provided.");

            // Check for existing entry
            var existing = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.ProfileId == request.ProfileId && ul.ShowId == dto.ShowId && ul.SeasonId == dto.SeasonId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("This item is already in your library.");

            var library = new UserLibrary
            {
                ProfileId = request.ProfileId,
                ShowId = dto.ShowId,
                SeasonId = dto.SeasonId,
                Status = dto.Status,
                EpisodesWatched = dto.EpisodesWatched,
                UserScore = dto.UserScore,
                StartedAt = dto.StartedAt,
                CompletedAt = dto.CompletedAt,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserLibraries.AddAsync(library, cancellationToken);
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
