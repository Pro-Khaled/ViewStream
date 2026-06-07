using MassTransit;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.MessageBus;

public class MassTransitMessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    public MassTransitMessageBus(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;
    public async Task Publish<T>(T message) where T : class => await _publishEndpoint.Publish(message);
}
