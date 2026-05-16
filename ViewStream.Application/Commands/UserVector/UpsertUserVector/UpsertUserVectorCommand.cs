using MediatR;

using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    public record UpsertUserVectorCommand(long ProfileId, CreateUpdateUserVectorDto Dto, long AdminUserId) : IRequest<bool>;
}
