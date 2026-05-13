using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface ICacheInvalidator
    {
        Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    }
}
