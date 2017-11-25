using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace Lab1_CRUD
{
    class DataAccess
    {
        public List<Director> GetDirector()
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                return connection.Query<Director>("SELECT * FROM Directors").ToList();
            }
        }
    }
}
