using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.CreateShow
{
    using Show = Domain.Entities.Show;

    public class CreateShowCommandHandler : IRequestHandler<CreateShowCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateShowCommandHandler> _logger;

        public CreateShowCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateShowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<long> Handle(CreateShowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating show: {Title}", request.Dto.Title);

            var show = _mapper.Map<Show>(request.Dto);

            if (request.Dto.GenreIds.Any())
            {
                var genres = await _unitOfWork.Genres.FindAsync(
                    g => request.Dto.GenreIds.Contains(g.Id), cancellationToken: cancellationToken);
                foreach (var genre in genres)
                    show.Genres.Add(genre);
            }

            if (request.Dto.TagIds.Any())
            {
                var tags = await _unitOfWork.ContentTags.FindAsync(
                    t => request.Dto.TagIds.Contains(t.Id), cancellationToken: cancellationToken);
                foreach (var tag in tags)
                    show.Tags.Add(tag);
            }

            await _unitOfWork.Shows.AddAsync(show, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Show, CreateShowDto>(
                tableName: "Shows",
                recordId: show.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.CreatedByUserId
            );

            _logger.LogInformation("Show created with Id: {ShowId}", show.Id);
            return show.Id;
        }
    }
}
