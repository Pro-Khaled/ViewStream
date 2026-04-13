using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.DeleteContentTag
{
    public class DeleteContentTagCommandHandler : IRequestHandler<DeleteContentTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContentTagCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteContentTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.ContentTags.GetByIdAsync<int>(request.Id, cancellationToken);
            if (tag == null) return false;

            _unitOfWork.ContentTags.Delete(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
