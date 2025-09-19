using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Retail.CheckList.DTOs.Responses.Common;

namespace Retail.CheckList.Infrastructure.IQuery
{
    public interface IBaseQuery<TResponse>
    {
        public Task<IEnumerable<TResponse>> GetAsync();
    }
}