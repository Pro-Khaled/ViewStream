using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserInteraction.CreateUserInteraction
{
    using UserInteraction = ViewStream.Domain.Entities.UserInteraction;
    public class CreateUserInteractionCommandHandler : IRequestHandler<CreateUserInteractionCommand, UserInteractionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateUserInteractionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserInteractionDto> Handle(CreateUserInteractionCommand request, CancellationToken cancellationToken)
        {
            var interaction = new UserInteraction
            {
                ProfileId = request.ProfileId,
                ShowId = request.Dto.ShowId,
                InteractionType = request.Dto.InteractionType,
                Weight = request.Dto.Weight ?? 1.0m,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserInteractions.AddAsync(interaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.UserInteractions.FindAsync(
                i => i.Id == interaction.Id,
                include: q => q.Include(i => i.Profile).Include(i => i.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserInteractionDto>(result.First());
        }
    }
}
