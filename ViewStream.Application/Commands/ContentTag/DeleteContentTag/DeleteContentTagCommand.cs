using MediatR;

namespace ViewStream.Application.Commands.ContentTag.DeleteContentTag
{
    public record DeleteContentTagCommand(int Id) : IRequest<bool>;

}
