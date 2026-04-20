using Serilog.Sinks.MSSqlServer;
using System.Data;

namespace ECommerce.WebAPI.Configurations.ColumnWriters
{
    public class UsernameColumnWriter : SqlColumn
    {
        public UsernameColumnWriter()
        {
            ColumnName = "user_name";
            DataType = SqlDbType.NVarChar;
            DataLength = 50;
            AllowNull = true;
        }
    }
}
