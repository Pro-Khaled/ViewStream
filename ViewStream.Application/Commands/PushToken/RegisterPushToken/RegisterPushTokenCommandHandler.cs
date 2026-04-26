using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PushToken.RegisterPushToken
{
    using PushToken = ViewStream.Domain.Entities.PushToken;
    public class RegisterPushTokenCommandHandler : IRequestHandler<RegisterPushTokenCommand, PushTokenDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RegisterPushTokenCommandHandler> _logger;

        public RegisterPushTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RegisterPushTokenCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PushTokenDto> Handle(RegisterPushTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering push token for UserId: {UserId}", request.UserId);

            var existing = await _unitOfWork.PushTokens.FindAsync(
                pt => pt.UserId == request.UserId && pt.DeviceId == request.Dto.DeviceId,
                cancellationToken: cancellationToken);

            var token = existing.FirstOrDefault();
            if (token != null)
            {
                token.Token = request.Dto.Token;
                token.Platform = request.Dto.Platform;
                token.LastUsed = DateTime.UtcNow;
                _unitOfWork.PushTokens.Update(token);
            }
            else
            {
                token = new PushToken
                {
                    UserId = request.UserId,
                    DeviceId = request.Dto.DeviceId,
                    Token = request.Dto.Token,
                    Platform = request.Dto.Platform,
                    LastUsed = DateTime.UtcNow
                };
                await _unitOfWork.PushTokens.AddAsync(token, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PushToken, object>(
                tableName: "PushTokens",
                recordId: token.Id,
                action: token == existing ? "UPDATE" : "INSERT",
                oldValues: null,
                newValues: new { token.UserId, token.DeviceId, token.Platform },
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Push token registered for UserId: {UserId}, DeviceId: {DeviceId}", request.UserId, request.Dto.DeviceId);
            return _mapper.Map<PushTokenDto>(token);
        }
    }
}
