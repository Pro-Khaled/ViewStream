using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.CreateShow
{
    using Show = Domain.Entities.Show;

    public class CreateShowCommandHandler : IRequestHandler<CreateShowCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateShowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateShowCommand request, CancellationToken cancellationToken)
        {
            var show = _mapper.Map<Show>(request.Dto);

            // Handle Genres
            if (request.Dto.GenreIds.Any())
            {
                var genres = await _unitOfWork.Genres.FindAsync(g => request.Dto.GenreIds.Contains(g.Id), cancellationToken: cancellationToken);
                foreach (var genre in genres)
                    show.Genres.Add(genre);
            }

            // Handle Tags
            if (request.Dto.TagIds.Any())
            {
                var tags = await _unitOfWork.ContentTags.FindAsync(t => request.Dto.TagIds.Contains(t.Id), cancellationToken: cancellationToken);
                foreach (var tag in tags)
                    show.Tags.Add(tag);
            }

            await _unitOfWork.Shows.AddAsync(show, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log (optional)
            // ...

            return show.Id;
        }
    }
}
