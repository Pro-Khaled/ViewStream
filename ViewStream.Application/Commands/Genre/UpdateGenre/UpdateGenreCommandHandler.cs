using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.UpdateGenre
{
    using Genre = ViewStream.Domain.Entities.Genre;
    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateGenreCommandHandler> _logger;

        public UpdateGenreCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateGenreCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating genre with Id: {GenreId}", request.Id);

            var genre = await _unitOfWork.Genres.GetByIdAsync<int>(request.Id, cancellationToken);
            if (genre == null)
            {
                _logger.LogWarning("Genre not found with Id: {GenreId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<GenreDto>(genre);
            _mapper.Map(request.Dto, genre);
            _unitOfWork.Genres.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Genre, object>(
                tableName: "Genres",
                recordId: genre.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Genre updated with Id: {GenreId}", genre.Id);
            return true;
        }
    }
}
