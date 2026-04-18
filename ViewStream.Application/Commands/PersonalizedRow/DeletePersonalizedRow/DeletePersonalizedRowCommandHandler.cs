using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow
{
    public class DeletePersonalizedRowCommandHandler : IRequestHandler<DeletePersonalizedRowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePersonalizedRowCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeletePersonalizedRowCommand request, CancellationToken cancellationToken)
        {
            var rows = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId && r.RowName == request.RowName,
                cancellationToken: cancellationToken);

            var row = rows.FirstOrDefault();
            if (row == null) return false;

            _unitOfWork.PersonalizedRows.Delete(row);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
