using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using MediatR;

namespace ViewStream.Application.Commands.SearchLog.PurgeOldSearchLogs
{
    public record PurgeOldSearchLogsCommand(int DaysToKeep, long AdminUserId) : IRequest<int>;
}
