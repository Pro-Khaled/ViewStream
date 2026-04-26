using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserInteraction.CreateUserInteraction
{
    using UserInteraction = ViewStream.Domain.Entities.UserInteraction;
    public class CreateUserInteractionCommandHandler : IRequestHandler<CreateUserInteractionCommand, UserInteractionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateUserInteractionCommandHandler> _logger;

        public CreateUserInteractionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateUserInteractionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserInteractionDto> Handle(CreateUserInteractionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging interaction: ProfileId={ProfileId}, ShowId={ShowId}, Type={Type}",
                request.ProfileId, request.Dto.ShowId, request.Dto.InteractionType);

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

            _logger.LogInformation("Interaction logged with Id: {InteractionId}", interaction.Id);

            var result = await _unitOfWork.UserInteractions.FindAsync(
                i => i.Id == interaction.Id,
                include: q => q.Include(i => i.Profile).Include(i => i.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserInteractionDto>(result.First());
        }
    }
}
