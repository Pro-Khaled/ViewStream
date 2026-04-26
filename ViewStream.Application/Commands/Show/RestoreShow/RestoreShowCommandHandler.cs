using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.RestoreShow
{
    using Show = ViewStream.Domain.Entities.Show;
    public class RestoreShowCommandHandler : IRequestHandler<RestoreShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreShowCommandHandler> _logger;

        public RestoreShowCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreShowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreShowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring show with Id: {ShowId}", request.Id);

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.Id, cancellationToken);
            if (show == null || show.IsDeleted != true)
            {
                _logger.LogWarning("Show not found or not deleted. Id: {ShowId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<ShowDto>(show);
            show.IsDeleted = false;
            show.DeletedAt = null;
            show.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Show, object>(
                tableName: "Shows",
                recordId: show.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.RestoredByUserId
            );

            _logger.LogInformation("Show restored. Id: {ShowId}", show.Id);
            return true;
        }
    }
}
