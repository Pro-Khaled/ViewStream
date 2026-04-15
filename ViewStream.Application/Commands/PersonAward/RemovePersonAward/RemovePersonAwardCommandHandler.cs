using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.RemovePersonAward
{
    public class RemovePersonAwardCommandHandler : IRequestHandler<RemovePersonAwardCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemovePersonAwardCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(RemovePersonAwardCommand request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == request.PersonId && pa.AwardId == request.AwardId, cancellationToken: cancellationToken);
            var item = items.FirstOrDefault();
            if (item == null) return false;
            _unitOfWork.PersonAwards.Delete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
