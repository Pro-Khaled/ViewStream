using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod
{
    public record SetDefaultPaymentMethodCommand(long Id, long UserId) : IRequest<bool>;

}
