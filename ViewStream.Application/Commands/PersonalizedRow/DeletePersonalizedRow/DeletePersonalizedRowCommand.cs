using MediatR;

namespace ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow
{
    public record DeletePersonalizedRowCommand(long ProfileId, string RowName) : IRequest<bool>;

}
