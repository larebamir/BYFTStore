using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthenticationExample
{
    public class AppDb : IDisposable
    {
        public MySqlConnection Connection { get; }

        public string ConnectionString { get; set; }

        public AppDb(string connectionString)
        {
            this.ConnectionString = connectionString;
            Connection = new MySqlConnection(connectionString);
        }

        public void Dispose() => Connection.Dispose();
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
