using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // Initiering av database-struktur - oppretter database og tabeller
    public class SequelInit
    {
        public MySqlConnection conn;
        public string dbConnString;

        public SequelInit(string dbIP, string dbname)
        {
            string rootConnString = $"Server={dbIP};Port={Constants.DataBasePort};User ID=root;Password={Constants.DataBaseRootPassword};";
            using (MySqlConnection rootConn = new MySqlConnection(rootConnString))
            {
                rootConn.Open();
                using (MySqlCommand cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{dbname}`;", rootConn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            dbConnString = $"Server={dbIP};Port={Constants.DataBasePort};Database={dbname};User ID=root;Password={Constants.DataBaseRootPassword};";
            conn = new MySqlConnection(dbConnString);
        }

        public bool TableExists(string tableName)
        {
            string query = "SHOW TABLES LIKE @tableName;";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        public void InitDb(bool DoMigration)
        {
            SequelMigrator seq = new SequelMigrator();
            seq.Open();

            List<string> TablesToCreate = new List<string> { "Users", "Markers", "RegisteredMarkers", "Notifications", "ReviewAssign" };
            foreach (var tblName in TablesToCreate)
            {
                if (!TableExists(tblName))
                {
                    switch (tblName)
                    {
                        case "Users":
                            seq.CreateTable(SequelTables.Users_Table(tblName), tblName);
                            break;
                        case "Markers":
                            seq.CreateTable(SequelTables.Markers_Table(tblName), tblName);
                            break;
                        case "RegisteredMarkers":
                            seq.CreateTable(SequelTables.RegisteredMarkers_Table(tblName), tblName);
                            break;
                        case "Notifications":
                            seq.CreateTable(SequelTables.Notifications_Table(tblName), tblName);
                            break;
                        case "ReviewAssign":
                            seq.CreateTable(SequelTables.ReviewAssign_Table(tblName), tblName);
                            break;
                    }
                }
            }
            seq.Close();

            if (DoMigration)
            {
                seq.Migrate();
            }
        }

        public void InitDb()
        {
            if (Constants.ResetDbOnStartup)
            {
                string[] TablesToRemove = {
                    "SET FOREIGN_KEY_CHECKS = 0;",
                    "DROP TABLE ReviewAssign;",
                    "DROP TABLE Notifications;",
                    "DROP TABLE RegisteredMarkers;",
                    "DROP TABLE Markers;",
                    "DROP TABLE Users;",
                    "SET FOREIGN_KEY_CHECKS = 1;"
                };

                foreach (var sql in TablesToRemove)
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("[SequelInit] Executed: " + sql);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[SequelInit] Failed (table not found): " + sql);
                    }
                }
            }

            // Create Users table if missing
            if (!TableExists("Users"))
            {
                string createUsers = @"
                    CREATE TABLE Users (
                        UserId        INT AUTO_INCREMENT PRIMARY KEY,
                        Name          VARCHAR(100) NOT NULL,
                        LastName      VARCHAR(100) NOT NULL,
                        FirstName     VARCHAR(100) NOT NULL,
                        UserType      ENUM('User','Admin','Employee') NOT NULL DEFAULT 'User',
                        Organization  VARCHAR(100),
                        CreatedAt     DATETIME DEFAULT CURRENT_TIMESTAMP,
                        NormalizedName     VARCHAR(100),
                        PasswordHash       VARCHAR(500),
                        Email              VARCHAR(255),
                        NormalizedEmail    VARCHAR(255),
                        SecurityStamp      VARCHAR(100),
                        ConcurrencyStamp   VARCHAR(100),
                        UserName           VARCHAR(100)
                    );";
                using (MySqlCommand cmd = new MySqlCommand(createUsers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Created Users Table");
                }

                // Test user for development
                string JohnnyTest = "INSERT INTO Users (Name,LastName) VALUES ('Johnny', 'Test')";
                using (MySqlCommand cmd = new MySqlCommand(JohnnyTest, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Added Test User");
                }
            }

            // Create Markers table if missing
            if (!TableExists("Markers"))
            {
                string createMarkers = @"
                    CREATE TABLE Markers (
                        MarkerId    INT AUTO_INCREMENT PRIMARY KEY,
                        Lat         DOUBLE NOT NULL,
                        Lng         DOUBLE NOT NULL,
                        Description VARCHAR(500),
                        UserId      INT NULL,
                        HeightMOverSea DECIMAL(6,2),
                        Type        VARCHAR(100) DEFAULT NULL,
                        Date        DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
                        GeoJson JSON
                    );";
                using (MySqlCommand cmd = new MySqlCommand(createMarkers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Created Markers Table");
                }
            }

            // Create RegisteredMarkers table if missing
            if (!TableExists("RegisteredMarkers"))
            {
                string createRegisteredMarkers = @"
                    CREATE TABLE RegisteredMarkers (
                        MarkerId          INT AUTO_INCREMENT PRIMARY KEY,
                        Lat               DOUBLE NOT NULL,
                        Lng               DOUBLE NOT NULL,
                        Description       VARCHAR(500),
                        UserId            INT NULL,
                        Organization      VARCHAR(100),
                        State             ENUM('Unseen','Seen','Rejected','Accepted') DEFAULT 'Unseen',
                        Type              VARCHAR(100),
                        HeightM           DECIMAL(6,2),
                        HeightMOverSea    DECIMAL(6,2),
                        AccuracyM         DECIMAL(5,2),
                        ObstacleCategory  VARCHAR(50),
                        IsTemporary       BOOLEAN DEFAULT FALSE,
                        Lighting          VARCHAR(100),
                        SubmittedBy       INT NULL,
                        ReviewedBy        INT NULL,
                        ReviewComment     VARCHAR(500),
                        LastUpdated       DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                        Source            VARCHAR(100),
                        Date              DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
                        FOREIGN KEY (SubmittedBy) REFERENCES Users(UserId) ON DELETE SET NULL,
                        FOREIGN KEY (ReviewedBy) REFERENCES Users(UserId) ON DELETE SET NULL,
                        GeoJson JSON
                    );";
                using (MySqlCommand cmd = new MySqlCommand(createRegisteredMarkers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Created RegisteredMarkers Table");
                }
            }

            // Create ReviewAssign table if missing
            if (!TableExists("ReviewAssign"))
            {
                string createReviewAssign = @"
                    CREATE TABLE ReviewAssign (
                        UserId INT NOT NULL,
                        MarkerId INT NOT NULL,
                        PRIMARY KEY (UserId, MarkerId),
                        FOREIGN KEY (MarkerId) REFERENCES RegisteredMarkers(MarkerId) ON DELETE CASCADE,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                    );";
                using (MySqlCommand cmd = new MySqlCommand(createReviewAssign, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Created ReviewAssign Table");
                }
            }

            // Create Notifications table if missing
            if (!TableExists("Notifications"))
            {
                string createNotifications = @"
                    CREATE TABLE Notifications (
                        NotificationId INT AUTO_INCREMENT PRIMARY KEY,
                        UserId INT NOT NULL,
                        MarkerId INT NULL,
                        Message VARCHAR(300) NOT NULL,
                        Date DATETIME DEFAULT CURRENT_TIMESTAMP,
                        IsRead BOOLEAN DEFAULT FALSE,
                        Type ENUM('Info', 'Warning', 'ReviewAssigned', 'MarkerAccepted', 'MarkerRejected') DEFAULT 'Info',
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                    );";
                using (MySqlCommand cmd = new MySqlCommand(createNotifications, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[SequelInit] Created Notifications Table");
                }
            }
        }
    }
}
