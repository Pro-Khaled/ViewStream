using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.UpdateContentTag
{
    public class UpdateContentTagCommandHandler : IRequestHandler<UpdateContentTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateContentTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateContentTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.ContentTags.GetByIdAsync<int>(request.Id, cancellationToken);
            if (tag == null) return false;

            _mapper.Map(request.Dto, tag);
            _unitOfWork.ContentTags.Update(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
