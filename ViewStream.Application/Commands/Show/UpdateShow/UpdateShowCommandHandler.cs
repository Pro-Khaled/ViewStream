using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UpdateShow
{
    public class UpdateShowCommandHandler : IRequestHandler<UpdateShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateShowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateShowCommand request, CancellationToken cancellationToken)
        {
            var shows = await _unitOfWork.Shows.FindAsync(
                predicate: s => s.Id == request.Id && s.IsDeleted != true,
                include: q => q.Include(s => s.Genres).Include(s => s.Tags),
                cancellationToken: cancellationToken);

            var show = shows.FirstOrDefault();
            if (show == null) return false;

            _mapper.Map(request.Dto, show);

            // Update Genres
            show.Genres.Clear();
            if (request.Dto.GenreIds.Any())
            {
                var genres = await _unitOfWork.Genres.FindAsync(g => request.Dto.GenreIds.Contains(g.Id), cancellationToken: cancellationToken);
                foreach (var genre in genres)
                    show.Genres.Add(genre);
            }

            // Update Tags
            show.Tags.Clear();
            if (request.Dto.TagIds.Any())
            {
                var tags = await _unitOfWork.ContentTags.FindAsync(t => request.Dto.TagIds.Contains(t.Id), cancellationToken: cancellationToken);
                foreach (var tag in tags)
                    show.Tags.Add(tag);
            }

            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
