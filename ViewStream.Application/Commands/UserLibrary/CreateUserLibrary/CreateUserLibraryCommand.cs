using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserLibrary.CreateUserLibrary
{
    public record CreateUserLibraryCommand(long ProfileId, CreateUserLibraryDto Dto) : IRequest<UserLibraryDto>;

}
