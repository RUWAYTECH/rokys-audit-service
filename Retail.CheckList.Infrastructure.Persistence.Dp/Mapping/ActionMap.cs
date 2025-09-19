using System;
using Dapper.FluentMap.Mapping;
using Retail.CheckList.DTOs.Responses.Action;

namespace Retail.CheckList.Infrastructure.Persistence.Dp.Mapping;

public class ActionMap : EntityMap<ActionResponseDto>
{
    public ActionMap()
    {
        Map(a => a.Action).ToColumn("Accion");
        Map(a => a.Description).ToColumn("Descripcion");
        Map(a => a.Status).ToColumn("Estado");
    }
}
