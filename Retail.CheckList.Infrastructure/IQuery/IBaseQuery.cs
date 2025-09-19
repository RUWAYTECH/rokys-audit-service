using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Infrastructure.IQuery
{
    public interface IBaseQuery<TResponse>
    {
        public Task<IEnumerable<TResponse>> GetAsync();
    }
}