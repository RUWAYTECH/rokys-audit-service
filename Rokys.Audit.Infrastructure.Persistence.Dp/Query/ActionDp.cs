using System.Data;
using Dapper;
using Rokys.Audit.DTOs.Responses.Action;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IQuery;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.Dp.Query
{
    public class ActionDp : IActionQuery
    {
         private readonly IDbConnection _dbConnection;
         public ActionDp(IDbConnection dbConnection)
         {
            _dbConnection = dbConnection;
         }

        public async Task<IEnumerable<ActionResponseDto>> GetAsync()
        {
            return await _dbConnection.QueryAsync<ActionResponseDto>("[Checklist].[sp_ListarAccion]", commandType: CommandType.StoredProcedure);
        }

    }
}