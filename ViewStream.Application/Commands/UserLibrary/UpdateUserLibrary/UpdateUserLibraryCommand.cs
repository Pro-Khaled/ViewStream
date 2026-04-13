using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary
{
    public record UpdateUserLibraryCommand(long Id, long ProfileId, UpdateUserLibraryDto Dto) : IRequest<UserLibraryDto?>;

}
