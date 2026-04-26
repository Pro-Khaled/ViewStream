using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Genre.CreateGenre
{
    using Genre = Domain.Entities.Genre;

    public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateGenreCommandHandler> _logger;

        public CreateGenreCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateGenreCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating genre: {GenreName}", request.Dto.Name);

            var genre = _mapper.Map<Genre>(request.Dto);
            await _unitOfWork.Genres.AddAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Genre, object>(
                tableName: "Genres",
                recordId: genre.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Genre created with Id: {GenreId}", genre.Id);
            return genre.Id;
        }
    }
}
