using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;

namespace KartverketRegister.Utils
{
    // baseklasse for mySQL-databaseforbindelser
    public class SequelBase : IDisposable
    {
        protected MySqlConnection conn;
        public string ConnectionString { get; protected set; }


        public SequelBase(string dbIP, string dbname)
        {
            ConnectionString = $"Server={dbIP};Port={Constants.DataBasePort};Database={dbname};User ID=root;Password={Constants.DataBaseRootPassword};";
            conn = new MySqlConnection(ConnectionString);
        }
        public SequelBase()
        {
            ConnectionString = $"Server={Constants.DataBaseIp};Port={Constants.DataBasePort};Database={Constants.DataBaseName};User ID=root;Password={Constants.DataBaseRootPassword};";
            conn = new MySqlConnection(ConnectionString);
        }
        public SequelBase(MySqlConnection connection)
        {
            conn = connection;
        }

        // Optional: Helper methods that all derived classes can use
        public void Open()
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        public void Close()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
        }

        // Implement IDisposable for safe cleanup
        public void Dispose()
        {
            if (conn != null)
            {
                conn.Dispose();
                conn = null;
            }
        }
        public MySqlConnection GetConnection()
        {
            return conn;
        }
    }
}