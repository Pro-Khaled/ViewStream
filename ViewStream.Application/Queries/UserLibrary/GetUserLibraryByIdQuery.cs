using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserLibrary
{
    public record GetUserLibraryByIdQuery(long Id, long ProfileId) : IRequest<UserLibraryDto?>;

}
