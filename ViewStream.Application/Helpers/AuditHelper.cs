using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Helpers
{
    public static class AuditHelper
    {
        public static void SetAudit<TEntity, TDto>(
            this IAuditContext context,
            string tableName,
            long recordId,
            string action,
            object? oldValues = null,
            object? newValues = null,
            long? changedByUserId = null)
        {
            context.TableName = tableName;
            context.RecordId = recordId;
            context.Action = action;
            context.OldValues = oldValues;
            context.NewValues = newValues;
            context.ChangedByUserId = changedByUserId;
        }
    }
}
