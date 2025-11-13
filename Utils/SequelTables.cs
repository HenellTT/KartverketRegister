namespace KartverketRegister.Utils
{
    public static class SequelTables
    {
        public static string Users_Table(string tableName)
        {
            return @$"
            CREATE TABLE {tableName} (
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
        }

        public static string Markers_Table(string tableName)
        {
            return @$"
            CREATE TABLE {tableName} (
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
        }

        public static string RegisteredMarkers_Table(string tableName)
        {
            return @$"
            CREATE TABLE {tableName} (
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
        }

        public static string ReviewAssign_Table(string tableName)
        {
            return @$"
            CREATE TABLE {tableName} (
                UserId INT NOT NULL,
                MarkerId INT NOT NULL,
                PRIMARY KEY (UserId, MarkerId),

                FOREIGN KEY (MarkerId) REFERENCES RegisteredMarkers(MarkerId) ON DELETE CASCADE,
                FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
            );";
        }

        public static string Notifications_Table(string tableName)
        {
            return @$"
            CREATE TABLE {tableName} (
                NotificationId INT AUTO_INCREMENT PRIMARY KEY,
                UserId INT NOT NULL,
                MarkerId INT NULL,
                Message VARCHAR(300) NOT NULL,
                Date DATETIME DEFAULT CURRENT_TIMESTAMP,
                IsRead BOOLEAN DEFAULT FALSE,
                Type ENUM('Info', 'Warning', 'ReviewAssigned', 'MarkerAccepted', 'MarkerRejected') DEFAULT 'Info',

                FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
            );";
        }
    }
}
