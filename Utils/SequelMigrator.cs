using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // Håndterer database-migrering - kopierer data til nye tabellstrukturer
    public class SequelMigrator : SequelBase
    {
        public SequelMigrator(string dbIP, string dbname) : base(dbIP, dbname) { }
        public SequelMigrator() : base() { }

        public void Migrate()
        {
            conn.Open();
            SetForeignKeyCheck(0);

            // Slett gamle kopier
            DropTable("Users_Copy");
            DropTable("Markers_Copy");
            DropTable("RegisteredMarkers_Copy");
            DropTable("Notifications_Copy");
            DropTable("ReviewAssign_Copy");

            // Opprett kopitabeller
            CreateTable(SequelTables.Users_Table("Users_Copy"), "Users_Copy");
            CreateTable(SequelTables.Markers_Table("Markers_Copy"), "Markers_Copy");
            CreateTable(SequelTables.RegisteredMarkers_Table("RegisteredMarkers_Copy"), "RegisteredMarkers_Copy");
            CreateTable(SequelTables.Notifications_Table("Notifications_Copy"), "Notifications_Copy");
            CreateTable(SequelTables.ReviewAssign_Table("ReviewAssign_Copy"), "ReviewAssign_Copy");

            // Kopier data
            CopyTableData("Users", "Users_Copy");
            CopyTableData("Markers", "Markers_Copy");
            CopyTableData("RegisteredMarkers", "RegisteredMarkers_Copy");
            CopyTableData("Notifications", "Notifications_Copy");
            CopyTableData("ReviewAssign", "ReviewAssign_Copy");

            // Slett originaler
            DropTable("Users");
            DropTable("Markers");
            DropTable("RegisteredMarkers");
            DropTable("Notifications");
            DropTable("ReviewAssign");

            // Opprett nye tabeller
            CreateTable(SequelTables.Users_Table("Users"), "Users");
            CreateTable(SequelTables.Markers_Table("Markers"), "Markers");
            CreateTable(SequelTables.RegisteredMarkers_Table("RegisteredMarkers"), "RegisteredMarkers");
            CreateTable(SequelTables.Notifications_Table("Notifications"), "Notifications");
            CreateTable(SequelTables.ReviewAssign_Table("ReviewAssign"), "ReviewAssign");

            // Kopier tilbake
            CopyTableData("Users_Copy", "Users");
            CopyTableData("Markers_Copy", "Markers");
            CopyTableData("RegisteredMarkers_Copy", "RegisteredMarkers");
            CopyTableData("Notifications_Copy", "Notifications");
            CopyTableData("ReviewAssign_Copy", "ReviewAssign");

            // Rydd opp
            DropTable("Users_Copy");
            DropTable("Markers_Copy");
            DropTable("RegisteredMarkers_Copy");
            DropTable("Notifications_Copy");
            DropTable("ReviewAssign_Copy");

            SetForeignKeyCheck(1);
            conn.Close();
        }

        public List<string> GetTableColumns(string tableName)
        {
            List<string> columns = new List<string>();
            string query = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @TableName
                ORDER BY ORDINAL_POSITION;";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
            }
            return columns;
        }

        public void CopyTableData(string oldTable, string newTable)
        {
            List<string> newColumns = GetTableColumns(newTable);
            List<string> oldColumns = GetTableColumns(oldTable);
            List<string> commonColumns = oldColumns.FindAll(c => newColumns.Contains(c));

            if (commonColumns.Count == 0)
                throw new Exception("No matching columns found between tables.");

            string columnList = string.Join(", ", commonColumns);
            string paramList = string.Join(", ", commonColumns.ConvertAll(c => "@" + c));
            string selectQuery = $"SELECT {columnList} FROM {oldTable}";

            using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
            {
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            string insertQuery = $"INSERT INTO {newTable} ({columnList}) VALUES ({paramList})";
                            using (MySqlConnection innerConn = new MySqlConnection(ConnectionString))
                            {
                                innerConn.Open();
                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, innerConn))
                                {
                                    foreach (var col in commonColumns)
                                    {
                                        insertCmd.Parameters.AddWithValue("@" + col, reader[col]);
                                    }
                                    insertCmd.ExecuteNonQuery();
                                    Console.WriteLine($"[SequelMigrator] Copied data from {oldTable} to {newTable}");
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"[SequelMigrator] Incompatible data from {oldTable} to {newTable}");
                        }
                    }
                }
            }
        }

        public void CreateTable(string SQL_Table, string tableName)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQL_Table, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Created {tableName}");
            }
        }

        public void DropTable(string tableName)
        {
            string sql = $"DROP TABLE IF EXISTS `{tableName}`;";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Dropped {tableName}");
            }
        }

        public void SetForeignKeyCheck(int value)
        {
            string sql = $"SET FOREIGN_KEY_CHECKS={value};";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
