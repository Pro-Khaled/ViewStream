using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode
{
    public record RedeemPromoCodeCommand(long UserId, string Code, string? PlanType) : IRequest<UserPromoUsageDto>;

}
