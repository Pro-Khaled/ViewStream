using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Profile
{
    public record GetProfileByIdQuery(long Id, long UserId) : IRequest<ProfileDto?>;

}
