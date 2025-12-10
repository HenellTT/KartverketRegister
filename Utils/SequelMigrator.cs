using MySql.Data;
using MySql.Data.MySqlClient;
namespace KartverketRegister.Utils
{

    // Klasse for migrering av database
    public class SequelMigrator : SequelBase
    {
        
        public SequelMigrator(string dbIP, string dbname) : base(dbIP, dbname) { }
        public SequelMigrator() : base() { }

        public void Migrate()
        {
            conn.Open();

            using (var transaction = conn.BeginTransaction())
            {
                Console.WriteLine("[SequelMigrator] Started Migration Transaction");
                try
                {
                    SetForeingKeyCheckTransaction(0, transaction);

                    DropTableTransaction("Users_Copy", transaction);
                    DropTableTransaction("Markers_Copy", transaction);
                    DropTableTransaction("RegisteredMarkers_Copy", transaction);
                    DropTableTransaction("Notifications_Copy", transaction);
                    DropTableTransaction("ReviewAssign_Copy", transaction);

                    CreateTableTransaction(SequelTables.Users_Table("Users_Copy"), "Users_Copy", transaction);
                    CreateTableTransaction(SequelTables.Markers_Table("Markers_Copy"), "Markers_Copy", transaction);
                    CreateTableTransaction(SequelTables.RegisteredMarkers_Table("RegisteredMarkers_Copy"), "RegisteredMarkers_Copy", transaction);
                    CreateTableTransaction(SequelTables.Notifications_Table("Notifications_Copy"), "Notifications_Copy", transaction);
                    CreateTableTransaction(SequelTables.ReviewAssign_Table("ReviewAssign_Copy"), "ReviewAssign_Copy", transaction);

                    CopyTableDataBulkTransaction("Users", "Users_Copy", transaction);
                    CopyTableDataBulkTransaction("Markers", "Markers_Copy", transaction);
                    CopyTableDataBulkTransaction("RegisteredMarkers", "RegisteredMarkers_Copy", transaction);
                    CopyTableDataBulkTransaction("Notifications", "Notifications_Copy", transaction);
                    CopyTableDataBulkTransaction("ReviewAssign", "ReviewAssign_Copy", transaction);

                    DropTableTransaction("Users", transaction);
                    DropTableTransaction("Markers", transaction);
                    DropTableTransaction("RegisteredMarkers", transaction);
                    DropTableTransaction("Notifications", transaction);
                    DropTableTransaction("ReviewAssign", transaction);

                    CreateTableTransaction(SequelTables.Users_Table("Users"), "Users", transaction);
                    CreateTableTransaction(SequelTables.Markers_Table("Markers"), "Markers", transaction);
                    CreateTableTransaction(SequelTables.RegisteredMarkers_Table("RegisteredMarkers"), "RegisteredMarkers", transaction);
                    CreateTableTransaction(SequelTables.Notifications_Table("Notifications"), "Notifications", transaction);
                    CreateTableTransaction(SequelTables.ReviewAssign_Table("ReviewAssign"), "ReviewAssign", transaction);

                    CopyTableDataBulkTransaction("Users_Copy", "Users", transaction);
                    CopyTableDataBulkTransaction("Markers_Copy", "Markers", transaction);
                    CopyTableDataBulkTransaction("RegisteredMarkers_Copy", "RegisteredMarkers", transaction);
                    CopyTableDataBulkTransaction("Notifications_Copy", "Notifications", transaction);
                    CopyTableDataBulkTransaction("ReviewAssign_Copy", "ReviewAssign", transaction);

                    DropTableTransaction("Users_Copy", transaction);
                    DropTableTransaction("Markers_Copy", transaction);
                    DropTableTransaction("RegisteredMarkers_Copy", transaction);
                    DropTableTransaction("Notifications_Copy", transaction);
                    DropTableTransaction("ReviewAssign_Copy", transaction);

                    SetForeingKeyCheckTransaction(1, transaction);

                    // Commit the transaction
                    transaction.Commit();
                }
                catch
                {
                    // Rollback if anything fails
                    Console.WriteLine("[SequelMigrator] Failed Migration Transaction");
                    transaction.Rollback();
                    Console.WriteLine("[SequelMigrator] Transaction Rolled Back");
                    throw;
                }
                finally
                {
                    Console.WriteLine("[SequelMigrator] Completed Migration Transaction");
                    conn.Close();
                }
            }
        }
        private List<string> GetTableColumns(string tableName)
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
        private void CopyTableDataBulk(string oldTable, string newTable)
        {
            var newColumns = GetTableColumns(newTable);
            var oldColumns = GetTableColumns(oldTable);
            var commonColumns = oldColumns.FindAll(c => newColumns.Contains(c));

            string sqlColumns = string.Join(", ", commonColumns);

            string sql = $"INSERT INTO {newTable} ({sqlColumns}) SELECT {sqlColumns} FROM {oldTable}";

            using (var cmd = new MySqlCommand(sql,conn))
            {
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"[SequelMigrator] Bulk copied data from {oldTable} to {newTable}");
        }
        private void CopyTableData(string oldTable, string newTable)
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
                        try
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
                        } catch
                        {
                            Console.WriteLine($"[SequelMigrator] COPY - INCOMPATIBLE DATA FROM {oldTable} TO {newTable}");

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
        public void CreateTableTransaction(string SQL_Table, string tableName, MySqlTransaction transaction)
        {
            using (var cmd = new MySqlCommand(SQL_Table, conn, transaction))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Created {tableName}");
            }
        }

        public void DropTableTransaction(string tableName, MySqlTransaction transaction)
        {
            string sqling = $"DROP TABLE IF EXISTS `{tableName}`;";
            using (var cmd = new MySqlCommand(sqling, conn, transaction))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"[SequelMigrator] Deleted {tableName}");
            }
        }

        public void SetForeingKeyCheckTransaction(int boolean, MySqlTransaction transaction)
        {
            string sqling = $"SET FOREIGN_KEY_CHECKS={boolean};";
            using (var cmd = new MySqlCommand(sqling, conn, transaction))
            {
                cmd.ExecuteNonQuery();
            }
        }
        public void CopyTableDataBulkTransaction(string oldTable, string newTable, MySqlTransaction transaction)
        {
            var newColumns = GetTableColumns(newTable);
            var oldColumns = GetTableColumns(oldTable);
            var commonColumns = oldColumns.FindAll(c => newColumns.Contains(c));

            if (!commonColumns.Any())
                throw new InvalidOperationException("No common columns found between tables.");

            string sqlColumns = string.Join(", ", commonColumns.Select(c => $"`{c}`"));
            string sql = $"INSERT INTO `{newTable}` ({sqlColumns}) SELECT {sqlColumns} FROM `{oldTable}`";

            using (var cmd = new MySqlCommand(sql, conn, transaction))
            {
                cmd.ExecuteNonQuery();
            }
        }

    }
}
