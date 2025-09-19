using System;
using Retail.CheckList.DTOs.Responses.Action;
using Retail.CheckList.DTOs.Responses.Common;
using Retail.CheckList.Infrastructure.IMapping;
using Retail.CheckList.Infrastructure.IQuery;
using Retail.CheckList.Services.Interfaces;

namespace Retail.CheckList.Services.Services;

public class ActionService : IActionService
{
    private readonly IActionQuery _actionQuery;
    private readonly IAMapper _mapper;
    public ActionService(IActionQuery actionQuery, IAMapper mapper)
    {
        _actionQuery = actionQuery;
        _mapper = mapper;
    }
    public async Task<ResponseDto<IEnumerable<ActionResponseDto>>> GetAsync()
    {
        var response = ResponseDto.Create<IEnumerable<ActionResponseDto>>();
        var entities = await _actionQuery.GetAsync();
        response.Data = entities;
        return response;
    }
}
