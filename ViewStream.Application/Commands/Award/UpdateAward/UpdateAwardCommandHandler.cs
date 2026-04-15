using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.UpdateAward
{
    public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, AwardDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AwardDto?> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
        {
            var award = await _unitOfWork.Awards.GetByIdAsync<int>(request.Id, cancellationToken);
            if (award == null) return null;
            _mapper.Map(request.Dto, award);
            _unitOfWork.Awards.Update(award);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AwardDto>(award);
        }
    }
}
