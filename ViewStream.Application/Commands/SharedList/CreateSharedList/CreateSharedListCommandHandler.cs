using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.CreateSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;
    public class CreateSharedListCommandHandler : IRequestHandler<CreateSharedListCommand, SharedListDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSharedListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SharedListDto> Handle(CreateSharedListCommand request, CancellationToken cancellationToken)
        {
            var list = new SharedList
            {
                OwnerProfileId = request.OwnerProfileId,
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                IsPublic = request.Dto.IsPublic ?? false,
                ShareCode = GenerateUniqueShareCode(),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SharedLists.AddAsync(list, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.SharedLists.FindAsync(
                l => l.Id == list.Id,
                include: q => q.Include(l => l.OwnerProfile).Include(l => l.SharedListItems),
                cancellationToken: cancellationToken);

            return _mapper.Map<SharedListDto>(result.First());
        }

        private string GenerateUniqueShareCode()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 12);
        }
    }
}
