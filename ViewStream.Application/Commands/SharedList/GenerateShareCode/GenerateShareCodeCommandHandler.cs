using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.GenerateShareCode
{
    public class GenerateShareCodeCommandHandler : IRequestHandler<GenerateShareCodeCommand, string?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenerateShareCodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string?> Handle(GenerateShareCodeCommand request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
                return null;

            list.ShareCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 12);

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return list.ShareCode;
        }
    }
}
