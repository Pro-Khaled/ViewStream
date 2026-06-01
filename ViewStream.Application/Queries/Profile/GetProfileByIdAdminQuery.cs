using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Profile
{
    public record GetProfileByIdAdminQuery(long Id) : IRequest<ProfileDto?>;
}
