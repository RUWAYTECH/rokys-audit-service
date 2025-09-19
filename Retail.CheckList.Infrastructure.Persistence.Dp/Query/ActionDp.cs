using System.Data;
using Dapper;
using Retail.CheckList.DTOs.Responses.Action;
using Retail.CheckList.DTOs.Responses.Common;
using Retail.CheckList.Infrastructure.IQuery;
using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.Persistence.Dp.Query
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