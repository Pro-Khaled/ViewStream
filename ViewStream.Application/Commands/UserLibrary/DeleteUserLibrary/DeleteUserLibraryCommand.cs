using MediatR;
using ViewStream.Application.Common;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary
{
    public record DeleteUserLibraryCommand(long Id, long ProfileId) : IRequest<bool>;

}
