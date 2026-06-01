using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Subscription
{
    public record GetSubscriptionByIdAdminQuery(long Id) : IRequest<AdminSubscriptionListItemDto?>;
}
