using System;
using System.Configuration;
using Npgsql;

namespace ServicioPrueba1.DataAccess
{
    public class PostgreSQLConexion
    {
        private string connectionString;

        public PostgreSQLConexion()
        {
            connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConnectionXD"].ConnectionString;
        }

        public NpgsqlConnection ObtenerConexion()
        {
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}