using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Award
{
    public record GetAwardByIdQuery(int Id) : IRequest<AwardDto?>;

}
