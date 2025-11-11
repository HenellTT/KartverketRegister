-- Enkel "Reset" for å slette tabeller og views hvis de finnes
DROP VIEW IF EXISTS vw_TopObstacleTypes;
DROP TABLE IF EXISTS ReviewAssign;
DROP TABLE IF EXISTS Notifications;
DROP TABLE IF EXISTS RegisteredMarkers;
DROP TABLE IF EXISTS Markers;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Organizations;
----------------------------


---- Opprettelse av Tabeller ----

-- Organizations (lookup)
CREATE TABLE IF NOT EXISTS Organizations (
    OrganizationId INT AUTO_INCREMENT PRIMARY KEY,
    OrgName VARCHAR(150) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Users
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(100) NULL,
    Name VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    FirstName VARCHAR(100),
    UserType ENUM('User','Admin','Employee') NOT NULL DEFAULT 'User',
    OrganizationId INT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    NormalizedName VARCHAR(100),
    PasswordHash VARCHAR(500),
    Email VARCHAR(255),
    NormalizedEmail VARCHAR(255),
    SecurityStamp VARCHAR(100),
    ConcurrencyStamp VARCHAR(100),
    UNIQUE KEY ux_users_email (Email),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(OrganizationId) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Markers (temporary user-submitted markers)
CREATE TABLE IF NOT EXISTS Markers (
    MarkerId INT AUTO_INCREMENT PRIMARY KEY,
    Lat DOUBLE NOT NULL,
    Lng DOUBLE NOT NULL,
    Description VARCHAR(1000),
    UserId INT NULL,
    HeightMOverSea DECIMAL(7,2),
    Type VARCHAR(100) DEFAULT NULL,
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    GeoJson JSON,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL
) ENGINE=InnoDB;

-- RegisteredMarkers (official reports)
CREATE TABLE IF NOT EXISTS RegisteredMarkers (
    MarkerId INT AUTO_INCREMENT PRIMARY KEY,
    Lat DOUBLE NOT NULL,
    Lng DOUBLE NOT NULL,
    Description VARCHAR(1000),
    UserId INT,                 -- owner or original submitter (nullable to preserve history)
    Organization VARCHAR(150),
    State ENUM('Unseen','Seen','Rejected','Accepted') DEFAULT 'Unseen',
    Type VARCHAR(100),
    HeightM DECIMAL(7,2),
    HeightMOverSea DECIMAL(7,2),
    AccuracyM DECIMAL(6,2),
    ObstacleCategory VARCHAR(80),
    IsTemporary BOOLEAN DEFAULT FALSE,
    Lighting VARCHAR(100),
    SubmittedBy INT,            -- user who submitted report
    ReviewedBy INT NULL,        -- reviewer user id
    ReviewComment VARCHAR(1000),
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    Source VARCHAR(100),
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    GeoJson JSON,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    FOREIGN KEY (SubmittedBy) REFERENCES Users(UserId) ON DELETE SET NULL,
    FOREIGN KEY (ReviewedBy) REFERENCES Users(UserId) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Review assignments
CREATE TABLE IF NOT EXISTS ReviewAssign (
    UserId INT NOT NULL,
    MarkerId INT NOT NULL,
    AssignedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (UserId, MarkerId),
    FOREIGN KEY (MarkerId) REFERENCES RegisteredMarkers(MarkerId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Notifications
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    MarkerId INT NULL,
    Message VARCHAR(1000) NOT NULL,
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsRead BOOLEAN DEFAULT FALSE,
    Type ENUM('Info', 'Warning', 'ReviewAssigned', 'MarkerAccepted', 'MarkerRejected') DEFAULT 'Info',
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (MarkerId) REFERENCES RegisteredMarkers(MarkerId) ON DELETE SET NULL
) ENGINE=InnoDB;

------------------------


---- Opprettelse av Indexes og Views ----
-- Indexes for optimalisering
CREATE INDEX idx_RegisteredMarkers_Date ON RegisteredMarkers(Date);
CREATE INDEX idx_RegisteredMarkers_SubmittedBy ON RegisteredMarkers(SubmittedBy);
CREATE INDEX idx_RegisteredMarkers_Type ON RegisteredMarkers(Type);
CREATE INDEX idx_RegisteredMarkers_State_Date ON RegisteredMarkers(State, Date);
CREATE INDEX idx_Markers_Date ON Markers(Date);
-- View - Topp hinder typer
CREATE VIEW vw_TopObstacleTypes AS
SELECT Type AS ObstacleType, COUNT(*) AS Occurrences
FROM RegisteredMarkers
GROUP BY Type
ORDER BY Occurrences DESC;
------------------------------------------


-- ------------------------
-- |     DUMMY DATA       | 
-- ------------------------

-- Organizations
INSERT INTO Organizations (OrgName) VALUES
('NLA'), ('Luftforsvaret'), ('Kartverket'), ('Politiets helikoptertjeneste'), ('Private');

-- Users (Dummy Data)
INSERT INTO Users (UserName, Name, LastName, FirstName, UserType, OrganizationId, Email, PasswordHash)
VALUES
('johnny', 'Johnny', 'Test', 'J.', 'User', 1, 'johnny@test.no', 'hash1'),
('kari', 'Kari', 'Hansen', 'K.', 'User', 2, 'kari@mil.no', 'hash2'),
('per', 'Per', 'Olsen', 'P.', 'User', 1, 'per@nla.no', 'hash3'),
('lars', 'Lars', 'Solberg', 'L.', 'User', 4, 'lars@politi.no', 'hash4'),
('svein', 'Svein', 'Admin', 'S.', 'Admin', 3, 'svein@kart.no', 'hash5'),
('anne', 'Anne', 'Monsen', 'A.', 'User', 1, 'anne@nla.no', 'hash6'),
('ola', 'Ola', 'Nordmann', 'O.', 'User', 2, 'ola@mil.no', 'hash7'),
('heidi', 'Heidi', 'Larsen', 'H.', 'User', 1, 'heidi@nla.no', 'hash8'),
('jonas', 'Jonas', 'Berg', 'J.', 'Employee', 3, 'jonas@kart.no', 'hash9'),
('eva', 'Eva', 'Jensen', 'E.', 'User', 4, 'eva@politi.no', 'hash10');

-- Temporary markers
INSERT INTO Markers (Lat, Lng, Description, UserId, HeightMOverSea, Type, Date)
VALUES
(58.1620, 8.0030, 'Old wind-turbine stump', 1, 120.50, 'WindTurbine', '2023-06-05 10:12:00'),
(58.1630, 8.0045, 'Tall spruce near helipad', 2, 50.25, 'Tree', '2024-03-22 15:00:00'),
(58.1640, 8.0055, 'Crane at construction site', 3, 15.00, 'Crane', '2024-09-01 09:30:00'),
(58.1651, 8.0067, 'Antenna mast', 4, 80.00, 'Antenna', '2025-02-11 11:00:00'),
(58.1662, 8.0078, 'Lighted tower', 5, 95.30, 'Tower', '2025-06-03 18:45:00'),
(58.1673, 8.0089, 'Temporary scaffold', 6, 12.50, 'Scaffold', '2023-11-08 07:20:00'),
(58.1684, 8.0090, 'Unknown obstacle', 7, 25.00, 'Unknown', '2024-07-14 12:00:00'),
(58.1695, 8.0011, 'Parachute training', 8, 5.00, 'Temporary', '2025-08-21 13:00:00'),
(58.1706, 8.0022, 'Old tower base', 9, 60.00, 'Tower', '2023-04-01 06:00:00'),
(58.1717, 8.0033, 'Tall mast near runway', 10, 40.00, 'Antenna', '2024-12-12 22:22:00'),
(58.1728, 8.0044, 'Lone birch', 1, 10.00, 'Tree', '2025-01-15 09:00:00'),
(58.1739, 8.0055, 'Rooftop AC unit', 2, 6.25, 'Structure', '2023-02-20 14:00:00');

-- RegisteredMarkers
INSERT INTO RegisteredMarkers
(Lat, Lng, Description, UserId, Organization, State, Type, HeightM, HeightMOverSea, AccuracyM, ObstacleCategory, IsTemporary, Lighting, SubmittedBy, ReviewedBy, ReviewComment, Date, Source)
VALUES
-- 2023 entries
(58.1001, 7.9001, 'Tree on ridge', 1, 'NLA', 'Accepted', 'Tree', 22.0, 150.0, 3.5, 'Natural', 0, 'None', 1, 5, 'ok', '2023-01-10 10:00:00', 'PilotReport'),
(58.1002, 7.9002, 'Crane near port', 2, 'Luftforsvaret', 'Accepted', 'Crane', 30.0, 160.0, 2.1, 'Construction', 1, 'None', 2, 5, 'ok', '2023-01-12 11:00:00', 'PilotReport'),
(58.1003, 7.9003, 'Antenna cluster', 3, 'NLA', 'Accepted', 'Antenna', 45.0, 170.0, 1.2, 'Infrastructure', 0, 'Lighted', 3, 5, 'ok', '2023-02-15 12:00:00', 'PilotReport'),
(58.1004, 7.9004, 'Old tower', 4, 'Politiets helikoptertjeneste', 'Rejected', 'Tower', 60.0, 200.0, 4.0, 'Infrastructure', 0, 'Lighted', 4, NULL, 'not verified', '2023-03-03 08:30:00', 'PilotReport'),
(58.1005, 7.9005, 'Small antenna', 5, 'Kartverket', 'Accepted', 'Antenna', 18.0, 110.0, 0.8, 'Infrastructure', 0, 'Lighted', 5, 5, 'ok', '2023-04-20 09:15:00', 'PilotReport'),
(58.1006, 7.9006, 'Barn roof obstruction', 6, 'NLA', 'Seen', 'Structure', 6.0, 105.0, 5.0, 'Building', 0, 'None', 6, NULL, '', '2023-05-05 16:40:00', 'PilotReport'),
(58.1007, 7.9007, 'Temporary scaffold', 2, 'Luftforsvaret', 'Seen', 'Scaffold', 14.0, 120.0, 2.5, 'Construction', 1, 'None', 7, NULL, '', '2023-06-10 07:10:00', 'PilotReport'),
(58.1008, 7.9008, 'Wind turbine', 8, 'NLA', 'Accepted', 'WindTurbine', 100.0, 250.0, 1.8, 'Energy', 0, 'Lighted', 8, 5, 'ok', '2023-07-20 19:00:00', 'PilotReport'),
(58.1009, 7.9009, 'Tower lights out', 5, 'Kartverket', 'Rejected', 'Tower', 70.0, 210.0, 6.0, 'Infrastructure', 0, 'None', 9, NULL, 'no lights', '2023-08-01 02:12:00', 'PilotReport'),
(58.1010, 7.9010, 'Crane at port', 10, 'Politiets helikoptertjeneste', 'Accepted', 'Crane', 28.0, 140.0, 1.5, 'Construction', 1, 'None', 10, 5, 'ok', '2023-09-01 10:10:00', 'PilotReport');

INSERT INTO RegisteredMarkers
(Lat, Lng, Description, UserId, Organization, State, Type, HeightM, HeightMOverSea, AccuracyM, ObstacleCategory, IsTemporary, Lighting, SubmittedBy, ReviewedBy, ReviewComment, Date, Source)
VALUES
-- 2024 entries
(58.1021, 7.9021, 'Crane at harbor', 1, 'NLA', 'Accepted', 'Crane', 33.0, 170.0, 1.5, 'Construction', 1, 'None', 1, 5, 'ok', '2024-01-20 10:00:00', 'PilotReport'),
(58.1022, 7.9022, 'High tension line', 2, 'Luftforsvaret', 'Accepted', 'Line', 12.0, 90.0, 3.0, 'Infrastructure', 0, 'None', 2, 5, 'ok', '2024-02-14 11:30:00', 'PilotReport'),
(58.1023, 7.9023, 'Antenna on hill', 3, 'NLA', 'Accepted', 'Antenna', 40.0, 160.0, 1.0, 'Infrastructure', 0, 'Lighted', 3, 5, 'ok', '2024-03-21 12:00:00', 'PilotReport'),
(58.1024, 7.9024, 'Temporary crane', 4, 'Politiets helikoptertjeneste', 'Seen', 'Crane', 20.0, 130.0, 2.3, 'Construction', 1, 'None', 4, NULL, '', '2024-04-01 09:00:00', 'PilotReport'),
(58.1025, 7.9025, 'Rooftop structure', 5, 'Kartverket', 'Accepted', 'Structure', 10.0, 95.0, 0.9, 'Building', 0, 'None', 5, 5, 'ok', '2024-05-06 14:00:00', 'PilotReport'),
(58.1026, 7.9026, 'Tall tree', 5, 'NLA', 'Accepted', 'Tree', 25.0, 180.0, 4.0, 'Natural', 0, 'None', 6, 5, 'ok', '2024-06-16 06:30:00', 'PilotReport'),
(58.1029, 7.9029, 'Temporary obstruction', 9, 'Kartverket', 'Rejected', 'Temporary', 5.0, 80.0, 7.0, 'Temporary', 1, 'None', 9, NULL, 'not valid', '2024-09-09 20:20:00', 'PilotReport'),
(58.1030, 7.9030, 'Tower near coast', 2, 'Politiets helikoptertjeneste', 'Accepted', 'Tower', 85.0, 220.0, 1.4, 'Infrastructure', 0, 'Lighted', 10, 5, 'ok', '2024-10-10 21:10:00', 'PilotReport');

INSERT INTO RegisteredMarkers
(Lat, Lng, Description, UserId, Organization, State, Type, HeightM, HeightMOverSea, AccuracyM, ObstacleCategory, IsTemporary, Lighting, SubmittedBy, ReviewedBy, ReviewComment, Date, Source)
VALUES
-- 2025 entries
(58.1101, 7.9101, 'New crane', 1, 'NLA', 'Accepted', 'Crane', 35.0, 175.0, 1.6, 'Construction', 1, 'None', 1, 5, 'ok', '2025-01-05 10:00:00', 'PilotReport'),
(58.1102, 7.9102, 'Tall antenna', 1, 'NLA', 'Accepted', 'Antenna', 60.0, 200.0, 0.9, 'Infrastructure', 0, 'Lighted', 1, 5, 'ok', '2025-02-11 11:00:00', 'PilotReport'),
(58.1103, 7.9103, 'Farm silo', 1, 'NLA', 'Accepted', 'Structure', 20.0, 110.0, 2.0, 'Building', 0, 'None', 1, 5, 'ok', '2025-03-18 12:00:00', 'PilotReport'),
(58.1104, 7.9104, 'Obstructing crane', 2, 'Luftforsvaret', 'Seen', 'Crane', 30.0, 150.0, 2.2, 'Construction', 1, 'None', 2, NULL, '', '2025-04-25 13:00:00', 'PilotReport'),
(58.1105, 7.9105, 'Ridge antenna', 3, 'NLA', 'Accepted', 'Antenna', 42.0, 165.0, 1.1, 'Infrastructure', 0, 'Lighted', 3, 5, 'ok', '2025-05-30 14:00:00', 'PilotReport'),
(58.1106, 7.9106, 'Wind turbine north', 4, 'Politiets helikoptertjeneste', 'Accepted', 'WindTurbine', 120.0, 260.0, 1.3, 'Energy', 0, 'Lighted', 4, 5, 'ok', '2025-06-15 15:00:00', 'PilotReport'),
(58.1107, 7.9107, 'Temporary lights', 1, 'NLA', 'Seen', 'Temporary', 6.0, 90.0, 3.0, 'Temporary', 1, 'None', 1, NULL, '', '2025-07-20 16:00:00', 'PilotReport');

-- Notifications (10 rows)
INSERT INTO Notifications (UserId, MarkerId, Message, Type)
VALUES
(1, 1, 'Your marker was accepted', 'MarkerAccepted'),
(2, 2, 'Please review assigned marker', 'ReviewAssigned'),
(3, 3, 'Marker rejected - missing data', 'MarkerRejected'),
(4, 4, 'General info', 'Info'),
(5, 5, 'New review assigned', 'ReviewAssigned'),
(6, 6, 'Reminder: pending review', 'Warning'),
(7, 7, 'Marker accepted', 'MarkerAccepted'),
(8, 8, 'Marker seen', 'Info'),
(9, 9, 'Marker rejected by reviewer', 'MarkerRejected'),
(10, 10, 'New tips available', 'Info');

INSERT INTO ReviewAssign (UserId, MarkerId) VALUES
(5, 1),
(5, 2),
(6, 3),
(6, 4),
(7, 5),
(7, 6),
(8, 7),
(8, 8),
(9, 9),
(9, 10);
-- End of SQL


-- Queries for testing:
-- 1. List opp de fem første radene av dei viktigste tabellene:
SELECT * FROM Users 
ORDER BY UserId 
LIMIT 5;

SELECT * FROM Organizations 
ORDER BY OrganizationId
LIMIT 5;

SELECT * 
FROM RegisteredMarkers 
ORDER BY Date 
DESC LIMIT 5;

SELECT * 
FROM Markers
ORDER BY Date
DESC LIMIT 5;

SELECT *
FROM Notifications
ORDER BY Date
DESC LIMIT 5;
---------------------------

-- 2. Piloter som har rapportert fleire enn 5 hinder.
SELECT u.UserId, u.Name, u.LastName, COUNT(r.MarkerId) AS ReportsCount
FROM Users u
JOIN RegisteredMarkers r ON r.SubmittedBy = u.UserId
GROUP BY u.UserId
HAVING COUNT(r.MarkerId) > 5
ORDER BY ReportsCount DESC;
---------------------------

-- 3. Gjennomsnitt på rapporterte hinder per år.
SELECT YEAR(Date) AS Yr, COUNT(*) AS TotalReports
FROM RegisteredMarkers
GROUP BY YEAR(Date)
ORDER BY Yr;
---------------------------

-- 4. Brukeren som rapporterte mest i 2025.
SELECT u.UserId, u.Name, u.LastName, COUNT(r.MarkerId) AS ReportsCount
FROM Users u
JOIN RegisteredMarkers r ON r.SubmittedBy = u.UserId
WHERE YEAR(r.Date) = 2025
GROUP BY u.UserId
ORDER BY ReportsCount DESC
LIMIT 1;
---------------------------

-- 5. Topp 3 hinder typer.
SELECT Type, COUNT(*) AS Occurences
FROM RegisteredMarkers
GROUP BY Type 
ORDER BY Occurences DESC
LIMIT 3;
---------------------------

-- 6. List opp alle piloter som har rapportert flere hindere i 2025 enn det som rapportertes inn i gjennomsnitt for år 2023.
WITH AvgReports2023 AS (
    SELECT AVG(ReportCount) AS AvgCount
    FROM (
        SELECT COUNT(*) AS ReportCount
        FROM RegisteredMarkers
        WHERE YEAR(Date) = 2023
        GROUP BY SubmittedBy
    ) AS Subquery
),
Reports2025 AS (
    SELECT SubmittedBy, COUNT(*) AS ReportCount
    FROM RegisteredMarkers
    WHERE YEAR(Date) = 2025
    GROUP BY SubmittedBy
)
SELECT u.UserId, u.Name, u.LastName, r.ReportCount
FROM Reports2025 r
JOIN Users u ON u.UserId = r.SubmittedBy
CROSS JOIN AvgReports2023 a
WHERE r.ReportCount > a.AvgCount
ORDER BY r.ReportCount DESC;
---------------------------
