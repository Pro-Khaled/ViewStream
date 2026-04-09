using MediatR;

namespace ViewStream.Application.Queries
{
    public abstract class BaseQuery<TResponse> : IRequest<TResponse>
    {
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    }
}
