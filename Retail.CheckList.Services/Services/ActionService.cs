using System;
using Rokys.Audit.DTOs.Responses.Action;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.IQuery;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.Services.Services;

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
