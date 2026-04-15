using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.CreateAward
{
    using Award = ViewStream.Domain.Entities.Award;
    public class CreateAwardCommandHandler : IRequestHandler<CreateAwardCommand, AwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AwardDto> Handle(CreateAwardCommand request, CancellationToken cancellationToken)
        {
            var award = _mapper.Map<Award>(request.Dto);
            await _unitOfWork.Awards.AddAsync(award, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AwardDto>(award);
        }
    }
}
