using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.DeleteWatchHistory
{
    public class DeleteWatchHistoryCommandHandler : IRequestHandler<DeleteWatchHistoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteWatchHistoryCommandHandler> _logger;

        public DeleteWatchHistoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteWatchHistoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteWatchHistoryCommand request, CancellationToken cancellationToken)
        {
            var history = await _unitOfWork.WatchHistories.GetByIdAsync<long>(request.Id, cancellationToken);
            if (history == null)
            {
                throw new NotFoundException("WatchHistory", request.Id);
            }

            _unitOfWork.WatchHistories.Delete(history);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Watch history entry hard-deleted with Id: {HistoryId}", history.Id);
            return true;
        }
    }
}

