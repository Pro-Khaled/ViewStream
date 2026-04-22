using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ErrorLog.CreateErrorLog
{
    public record CreateErrorLogCommand(CreateErrorLogDto Dto) : IRequest<ErrorLogDto>;

}
