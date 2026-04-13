using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.DeleteSubtitle
{
    public class DeleteSubtitleCommandHandler : IRequestHandler<DeleteSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubtitleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSubtitleCommand request, CancellationToken cancellationToken)
        {
            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted == true)
                return false;

            subtitle.IsDeleted = true;
            subtitle.DeletedAt = DateTime.UtcNow;
            subtitle.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
