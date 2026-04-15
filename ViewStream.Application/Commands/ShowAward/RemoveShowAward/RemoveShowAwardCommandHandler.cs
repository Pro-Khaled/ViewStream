using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAward.RemoveShowAward
{
    public class RemoveShowAwardCommandHandler : IRequestHandler<RemoveShowAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoveShowAwardCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(RemoveShowAwardCommand request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == request.ShowId && sa.AwardId == request.AwardId, cancellationToken: cancellationToken);
            var item = items.FirstOrDefault();
            if (item == null) return false;
            _unitOfWork.ShowAwards.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
