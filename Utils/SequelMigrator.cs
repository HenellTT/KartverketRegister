using MySql.Data;
using MySql.Data.MySqlClient;
namespace KartverketRegister.Utils
{
    public class SequelMigrator : SequelBase
    {
        public SequelMigrator(string dbIP, string dbname) : base(dbIP, dbname) { }
        public SequelMigrator() : base() { }

        public void Migrate()
        {
            conn.Open();
            SetForeingKeyCheck(0);

            DropTable("Users_Copy");
            DropTable("Markers_Copy");
            DropTable("RegisteredMarkers_Copy");
            DropTable("Notifications_Copy");
            DropTable("ReviewAssign_Copy");

            CreateTable(SequelTables.Users_Table("Users_Copy"), "Users_Copy");
            CreateTable(SequelTables.Markers_Table("Markers_Copy"), "Markers_Copy");
            CreateTable(SequelTables.RegisteredMarkers_Table("RegisteredMarkers_Copy"), "RegisteredMarkers_Copy");
            CreateTable(SequelTables.Notifications_Table("Notifications_Copy"), "Notifications_Copy");
            CreateTable(SequelTables.ReviewAssign_Table("ReviewAssign_Copy"), "ReviewAssign_Copy");

            CopyTableData("Users", "Users_Copy");
            CopyTableData("Markers", "Markers_Copy");
            CopyTableData("RegisteredMarkers", "RegisteredMarkers_Copy");
            CopyTableData("Notifications", "Notifications_Copy");
            CopyTableData("ReviewAssign", "ReviewAssign_Copy");

            DropTable("Users");
            DropTable("Markers");
            DropTable("RegisteredMarkers");
            DropTable("Notifications");
            DropTable("ReviewAssign");

            CreateTable(SequelTables.Users_Table("Users"), "Users");
            CreateTable(SequelTables.Markers_Table("Markers"), "Markers");
            CreateTable(SequelTables.RegisteredMarkers_Table("RegisteredMarkers"), "RegisteredMarkers");
            CreateTable(SequelTables.Notifications_Table("Notifications"), "Notifications");
            CreateTable(SequelTables.ReviewAssign_Table("ReviewAssign"), "ReviewAssign");

            CopyTableData("Users_Copy", "Users");
            CopyTableData("Markers_Copy", "Markers");
            CopyTableData("RegisteredMarkers_Copy", "RegisteredMarkers");
            CopyTableData("Notifications_Copy", "Notifications");
            CopyTableData("ReviewAssign_Copy", "ReviewAssign");

            DropTable("Users_Copy");
            DropTable("Markers_Copy");
            DropTable("RegisteredMarkers_Copy");
            DropTable("Notifications_Copy");
            DropTable("ReviewAssign_Copy");

            SetForeingKeyCheck(1);

            conn.Close();
        }
        public List<string> GetTableColumns(string tableName)
        {
            var columns = new List<string>();

            try
            {

                string query = @"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @TableName
            ORDER BY ORDINAL_POSITION;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader.GetString(0)); // Use ordinal 0 for COLUMN_NAME
                        }
                    }
                }
            } finally
            {

            }
            

            return columns;
        }
        public void CopyTableData(string oldTable, string newTable)
        {
            var newColumns = GetTableColumns(newTable);
            var oldColumns = GetTableColumns(oldTable); 
            var commonColumns = oldColumns.FindAll(c => newColumns.Contains(c));
            if (commonColumns.Count == 0)
                throw new Exception("No matching columns found between tables.");

            string columnList = string.Join(", ", commonColumns);
            string paramList = string.Join(", ", commonColumns.ConvertAll(c => "@" + c));

            string selectQuery = $"SELECT {columnList} FROM {oldTable}";

            using (var selectCmd = new MySqlCommand(selectQuery, conn))
            {
                
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string insertQuery = $"INSERT INTO {newTable} ({columnList}) VALUES ({paramList})";
                        using (var innerConn = new MySqlConnection(ConnectionString))
                        {
                            innerConn.Open();
                            using (var insertCmd = new MySqlCommand(insertQuery, innerConn))
                            {
                                foreach (var col in commonColumns)
                                {
                                    insertCmd.Parameters.AddWithValue("@" + col, reader[col]);
                                }
                                insertCmd.ExecuteNonQuery();
                                Console.WriteLine($"[SequelMigrator] Copied data from {oldTable} to {newTable}");
                            }
                            innerConn.Close();
                        }

                    }
                } 
            }
        }
        public void CreateTable(string SQL_Table, string tableName)
        {
            using (var cmd = new MySqlCommand(SQL_Table, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Created {tableName}");
            }
        }
        public void DropTable(string tableName)
        {
            string sqling = $"DROP TABLE IF EXISTS `{tableName}`;";
            using (var cmd = new MySqlCommand(sqling, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Deleted {tableName}");
            }
        }
        public void SetForeingKeyCheck(int boolean) {
            string sqling = $"SET FOREIGN_KEY_CHECKS={boolean};";
            using (var cmd = new MySqlCommand(sqling, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
