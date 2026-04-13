using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory
{
    using WatchHistory = ViewStream.Domain.Entities.WatchHistory;
    public class UpsertWatchHistoryCommandHandler : IRequestHandler<UpsertWatchHistoryCommand, WatchHistoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertWatchHistoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WatchHistoryDto> Handle(UpsertWatchHistoryCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.WatchHistories.FindAsync(
                wh => wh.ProfileId == request.ProfileId && wh.EpisodeId == request.Dto.EpisodeId,
                cancellationToken: cancellationToken);

            var history = existing.FirstOrDefault();
            if (history == null)
            {
                history = new WatchHistory
                {
                    ProfileId = request.ProfileId,
                    EpisodeId = request.Dto.EpisodeId,
                    ProgressSeconds = request.Dto.ProgressSeconds ?? 0,
                    Completed = request.Dto.Completed ?? false,
                    WatchedAt = DateTime.UtcNow
                };
                await _unitOfWork.WatchHistories.AddAsync(history, cancellationToken);
            }
            else
            {
                history.ProgressSeconds = request.Dto.ProgressSeconds ?? history.ProgressSeconds;
                history.Completed = request.Dto.Completed ?? history.Completed;
                history.WatchedAt = DateTime.UtcNow;
                _unitOfWork.WatchHistories.Update(history);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.WatchHistories.FindAsync(
                wh => wh.Id == history.Id,
                include: q => q.Include(wh => wh.Profile)
                               .Include(wh => wh.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchHistoryDto>(result.First());
        }
    }
}
