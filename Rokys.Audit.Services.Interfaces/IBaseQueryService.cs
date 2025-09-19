using System;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces;

public interface IBaseQueryService<TResponse>
{       
     public Task<ResponseDto<IEnumerable<TResponse>>> GetAsync();

}
