using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.RestoreSubtitle
{
    public class RestoreSubtitleCommandHandler : IRequestHandler<RestoreSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreSubtitleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreSubtitleCommand request, CancellationToken cancellationToken)
        {
            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted != true)
                return false;

            subtitle.IsDeleted = false;
            subtitle.DeletedAt = null;
            subtitle.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
