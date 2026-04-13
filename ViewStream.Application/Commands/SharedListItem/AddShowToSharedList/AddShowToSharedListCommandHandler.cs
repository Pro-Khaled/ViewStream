using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.AddShowToSharedList
{
    using SharedListItem = ViewStream.Domain.Entities.SharedListItem;
    public class AddShowToSharedListCommandHandler : IRequestHandler<AddShowToSharedListCommand, SharedListItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddShowToSharedListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SharedListItemDto> Handle(AddShowToSharedListCommand request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.SharedLists.GetByIdAsync<long>(request.ListId, cancellationToken);
            if (list == null || list.IsDeleted == true)
                throw new InvalidOperationException("List not found.");

            // Permission check
            if (list.OwnerProfileId != request.ProfileId && list.IsPublic != true)
                throw new UnauthorizedAccessException("You don't have permission to add items to this list.");

            var existing = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == request.ListId && i.ShowId == request.Dto.ShowId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("Show is already in this list.");

            var item = new SharedListItem
            {
                ListId = request.ListId,
                ShowId = request.Dto.ShowId,
                AddedByProfileId = request.ProfileId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.SharedListItems.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.SharedListItems.FindAsync(
                i => i.ListId == item.ListId && i.ShowId == item.ShowId,
                include: q => q.Include(i => i.List).Include(i => i.Show).Include(i => i.AddedByProfile),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListItemDto>(result.First());
        }
    }
}
