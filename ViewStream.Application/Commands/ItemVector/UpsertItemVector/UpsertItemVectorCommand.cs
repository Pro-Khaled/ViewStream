using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    public record UpsertItemVectorCommand(long ShowId, CreateUpdateItemVectorDto Dto) : IRequest<ItemVectorDto>;

}
