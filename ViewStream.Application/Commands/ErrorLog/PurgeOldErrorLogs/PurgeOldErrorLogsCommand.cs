using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using MediatR;

namespace ViewStream.Application.Commands.ErrorLog.PurgeOldErrorLogs
{
    public record PurgeOldErrorLogsCommand(int DaysToKeep, long AdminUserId) : IRequest<int>;
}
