using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;

namespace KartverketRegister.Utils
{
    public abstract class SequelBase : IDisposable
    {
        protected MySqlConnection conn;


        protected SequelBase(string dbIP, string dbname)
        {
            string dbConnString = $"Server={dbIP};Port={Constants.DataBasePort};Database={dbname};User ID=root;Password={Constants.DataBaseRootPassword};";
            conn = new MySqlConnection(dbConnString);
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
    }
}