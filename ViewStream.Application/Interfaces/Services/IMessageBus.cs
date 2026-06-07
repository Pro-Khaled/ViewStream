namespace ViewStream.Application.Interfaces.Services;

public interface IMessageBus
{
    Task Publish<T>(T message) where T : class;
}
 