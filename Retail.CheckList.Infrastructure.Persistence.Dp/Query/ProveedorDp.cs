using Dapper;
using Retail.CheckList.Model.Tables;
using System.Data;
using Retail.CheckList.Infrastructure.IQuery;

namespace Retail.CheckList.Infrastructure.Persistence.Dp.Query
{
    
    public class ProveedorDp: IProveedorQuery
    {
        private readonly IDbConnection _dbConnection;
        public ProveedorDp(IDbConnection dbConnection)
        {
            _dbConnection= dbConnection;
        }

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            var sql = "SELECT * FROM Proveedor";
            return await _dbConnection.QueryAsync<Proveedor>(sql);
        }

    }
}
