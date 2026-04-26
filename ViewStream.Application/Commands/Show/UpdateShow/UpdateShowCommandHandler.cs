using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UpdateShow
{
    using Show = ViewStream.Domain.Entities.Show;
    public class UpdateShowCommandHandler : IRequestHandler<UpdateShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateShowCommandHandler> _logger;

        public UpdateShowCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateShowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateShowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating show with Id: {ShowId}", request.Id);

            var shows = await _unitOfWork.Shows.FindAsync(
                predicate: s => s.Id == request.Id && s.IsDeleted != true,
                include: q => q.Include(s => s.Genres).Include(s => s.Tags),
                cancellationToken: cancellationToken);

            var show = shows.FirstOrDefault();
            if (show == null)
            {
                _logger.LogWarning("Show not found or already deleted. Id: {ShowId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ShowDto>(show);
            _mapper.Map(request.Dto, show);

            // Update Genres
            show.Genres.Clear();
            if (request.Dto.GenreIds.Any())
            {
                var genres = await _unitOfWork.Genres.FindAsync(
                    g => request.Dto.GenreIds.Contains(g.Id), cancellationToken: cancellationToken);
                foreach (var genre in genres)
                    show.Genres.Add(genre);
            }

            // Update Tags
            show.Tags.Clear();
            if (request.Dto.TagIds.Any())
            {
                var tags = await _unitOfWork.ContentTags.FindAsync(
                    t => request.Dto.TagIds.Contains(t.Id), cancellationToken: cancellationToken);
                foreach (var tag in tags)
                    show.Tags.Add(tag);
            }

            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Show, UpdateShowDto>(
                tableName: "Shows",
                recordId: show.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UpdatedByUserId
            );

            _logger.LogInformation("Show updated. Id: {ShowId}", show.Id);
            return true;
        }
    }
}
