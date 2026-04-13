using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.UpdateSharedList
{
    public class UpdateSharedListCommandHandler : IRequestHandler<UpdateSharedListCommand, SharedListDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSharedListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SharedListDto?> Handle(UpdateSharedListCommand request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.Id, cancellationToken);
            if (list == null || list.OwnerProfileId != request.OwnerProfileId || list.IsDeleted == true)
                return null;

            list.Name = request.Dto.Name;
            list.Description = request.Dto.Description;
            list.IsPublic = request.Dto.IsPublic;

            _unitOfWork.SharedLists.Update(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.SharedLists.FindAsync(
                l => l.Id == list.Id,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListDto>(result.First());
        }
    }
}
