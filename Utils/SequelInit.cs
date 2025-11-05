
using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace KartverketRegister.Utils
{
	// initiering av database struktur, Ikke noe mer, Bare det. Lager Database navn, og Sjekker om tabeller eksisterer og lager dem om de ikke gjør det.
    public class SequelInit
    {
        public MySqlConnection conn;
        public string dbConnString;

        public SequelInit(string dbIP, string dbname)
        {

            string rootConnString = $"Server={dbIP};Port={Constants.DataBasePort};User ID=root;Password={Constants.DataBaseRootPassword};";
            using (var rootConn = new MySqlConnection(rootConnString))
            {
                rootConn.Open();
                using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{dbname}`;", rootConn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Step 2: Initialize class-level connection to the target database
            dbConnString = $"Server={dbIP};Port={Constants.DataBasePort};Database={dbname};User ID=root;Password={Constants.DataBaseRootPassword};";
            conn = new MySqlConnection(dbConnString);

        }
        public bool TableExists(string tableName)
        {
            string query = "SHOW TABLES LIKE @tableName;";
            bool result = false;
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                using (var reader = cmd.ExecuteReader())
                {
                    result = reader.HasRows;
                }
            }
            return result;

        }
        public void InitDb()
        {
            if (Constants.ResetDbOnStartup)
            {
                string DropUsers = @"DROP TABLE Users;";
                string DropMarkers = @"DROP TABLE Markers;";
                string DropRegisteredMarkers = @"DROP TABLE RegisteredMarkers;";
                string[] TablesToRemove =  { DropRegisteredMarkers, DropMarkers, DropUsers };

                for (int i = 0; i < TablesToRemove.Length; i++)
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(TablesToRemove[i], conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Mysql Executed: " + TablesToRemove[i]);
                        }
                    } catch
                    {
                        Console.WriteLine("Command failed, table not found: " + TablesToRemove[i]);
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
                UserType      ENUM('User','Admin') NOT NULL DEFAULT 'User',
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
                using (var cmd = new MySqlCommand(createUsers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SequelInit: Created Users Table");
                }
                // Johnny Test er bare en Test dummy bruker som er laget for utvikling!!
                string JohnnyTest = "INSERT INTO Users (Name,LastName) VALUES ('Johnny', 'Test')";
                using (var cmd = new MySqlCommand(JohnnyTest, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SequelInit: Added Test User");
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
                HeightMOverSea    DECIMAL(6,2),
                Type        VARCHAR(100) DEFAULT NULL,
                Date        DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,

                GeoJson     JSON
            );";
                using (var cmd = new MySqlCommand(createMarkers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SequelInit: Created Markers Table");
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
                UserId            INT,
                Organization      VARCHAR(100),
                State             ENUM('Unseen','Seen','Rejected','Accepted') DEFAULT 'Unseen',
                Type              VARCHAR(100),
                HeightM           DECIMAL(6,2),
                HeightMOverSea    DECIMAL(6,2),
                AccuracyM         DECIMAL(5,2),
                ObstacleCategory  VARCHAR(50),
                IsTemporary       BOOLEAN DEFAULT FALSE,
                Lighting          VARCHAR(100),
                SubmittedBy       INT,
                ReviewedBy        INT NULL,
                ReviewComment     VARCHAR(500),
                LastUpdated       DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                Source            VARCHAR(100),
                Date              DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
                FOREIGN KEY (SubmittedBy) REFERENCES Users(UserId) ON DELETE SET NULL,
                FOREIGN KEY (ReviewedBy) REFERENCES Users(UserId) ON DELETE SET NULL,

                GeoJson           JSON
            );";
                using (var cmd = new MySqlCommand(createRegisteredMarkers, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SequelInit: Created RegisteredMarkers Table");
                }
            }
        }

    }
}