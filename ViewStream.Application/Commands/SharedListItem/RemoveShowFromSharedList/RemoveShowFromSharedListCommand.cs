using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList
{
    public record RemoveShowFromSharedListCommand(long ListId, long ShowId, long ProfileId) : IRequest<bool>;

}
