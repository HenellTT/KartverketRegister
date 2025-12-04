using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // Baseklasse for alle database-tilgangslag
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
            ConnectionString = connection?.ConnectionString ?? "";
        }

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

        public void Dispose()
        {
            conn?.Dispose();
            conn = null;
        }

        public MySqlConnection GetConnection() => conn;
    }
}
