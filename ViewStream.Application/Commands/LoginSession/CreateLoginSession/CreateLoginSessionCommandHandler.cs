using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.CreateLoginSession
{
    using LoginSession = ViewStream.Domain.Entities.LoginSession;
    public class CreateLoginSessionCommandHandler : IRequestHandler<CreateLoginSessionCommand, LoginSessionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateLoginSessionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoginSessionDto> Handle(CreateLoginSessionCommand request, CancellationToken cancellationToken)
        {
            var session = new LoginSession
            {
                UserId = request.UserId,
                DeviceId = request.DeviceId,
                SessionToken = request.SessionToken,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt
            };

            await _unitOfWork.LoginSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.LoginSessions.FindAsync(
                s => s.Id == session.Id,
                include: q => q.Include(s => s.Device),
                cancellationToken: cancellationToken);

            return _mapper.Map<LoginSessionDto>(result.First());
        }
    }
}
