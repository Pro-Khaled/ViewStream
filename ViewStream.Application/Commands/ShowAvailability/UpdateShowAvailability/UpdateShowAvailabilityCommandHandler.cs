using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability
{

    public class UpdateShowAvailabilityCommandHandler : IRequestHandler<UpdateShowAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateShowAvailabilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var availability = await _unitOfWork.ShowAvailabilities.FindAsync(
                predicate: sa => sa.ShowId == request.ShowId && sa.CountryCode == request.CountryCode,
                cancellationToken: cancellationToken);

            var entity = availability.FirstOrDefault();
            if (entity == null) return false;

            _mapper.Map(request.Dto, entity);
            _unitOfWork.ShowAvailabilities.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
