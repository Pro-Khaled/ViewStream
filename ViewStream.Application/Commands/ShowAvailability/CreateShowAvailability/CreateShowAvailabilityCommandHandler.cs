using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability
{
    using ShowAvailability = Domain.Entities.ShowAvailability;
    public class CreateShowAvailabilityCommandHandler : IRequestHandler<CreateShowAvailabilityCommand, (long ShowId, string CountryCode)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateShowAvailabilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(long ShowId, string CountryCode)> Handle(CreateShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var availability = _mapper.Map<ShowAvailability>(request.Dto);
            await _unitOfWork.ShowAvailabilities.AddAsync(availability, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return (availability.ShowId, availability.CountryCode);
        }
    }
}
