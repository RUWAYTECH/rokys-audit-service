using System;
using Retail.CheckList.DTOs.Responses.Common;

namespace Retail.CheckList.Services.Interfaces;

public interface IBaseQueryService<TResponse>
{       
     public Task<ResponseDto<IEnumerable<TResponse>>> GetAsync();

}
