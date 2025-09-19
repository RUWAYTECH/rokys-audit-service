using Dapper;
using Rokys.Audit.Model.Tables;
using System.Data;
using Rokys.Audit.Infrastructure.IQuery;

namespace Rokys.Audit.Infrastructure.Persistence.Dp.Query
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
