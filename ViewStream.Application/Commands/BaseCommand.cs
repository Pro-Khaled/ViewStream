using MediatR;

namespace ViewStream.Application.Commands
{
    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    }
}
