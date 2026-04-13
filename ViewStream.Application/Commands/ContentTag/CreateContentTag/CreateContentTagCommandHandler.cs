using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.CreateContentTag
{
    using ContentTag = Domain.Entities.ContentTag;

    public class CreateContentTagCommandHandler : IRequestHandler<CreateContentTagCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateContentTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateContentTagCommand request, CancellationToken cancellationToken)
        {
            var tag = _mapper.Map<ContentTag>(request.Dto);
            await _unitOfWork.ContentTags.AddAsync(tag, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return tag.Id;
        }
    }
}
