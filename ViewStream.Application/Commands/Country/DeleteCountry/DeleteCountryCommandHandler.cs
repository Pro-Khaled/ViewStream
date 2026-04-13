using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.DeleteCountry
{
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCountryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync<string>(request.Code, cancellationToken);
            if (country == null) return false;

            _unitOfWork.Countries.Delete(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
