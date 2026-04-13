using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList
{
    public class RemoveShowFromSharedListCommandHandler : IRequestHandler<RemoveShowFromSharedListCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveShowFromSharedListCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RemoveShowFromSharedListCommand request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == request.ListId && i.ShowId == request.ShowId,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            if (item == null) return false;

            // Check list ownership
            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.ListId, cancellationToken);
            if (list == null || (list.OwnerProfileId != request.ProfileId && item.AddedByProfileId != request.ProfileId))
                return false;

            _unitOfWork.SharedListItems.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
