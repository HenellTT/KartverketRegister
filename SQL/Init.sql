-- Create Users table
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    UserType ENUM('User','Admin','Employee') NOT NULL DEFAULT 'User',
    Organization VARCHAR(100),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    NormalizedName VARCHAR(100),
    PasswordHash VARCHAR(500),
    Email VARCHAR(255),
    NormalizedEmail VARCHAR(255),
    SecurityStamp VARCHAR(100),
    ConcurrencyStamp VARCHAR(100),
    UserName VARCHAR(100)
);

-- Create Markers table
CREATE TABLE IF NOT EXISTS Markers (
    MarkerId INT AUTO_INCREMENT PRIMARY KEY,
    Lat DOUBLE NOT NULL,
    Lng DOUBLE NOT NULL,
    Description VARCHAR(500),
    UserId INT NULL,
    HeightMOverSea DECIMAL(6,2),
    Type VARCHAR(100) DEFAULT NULL,
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    GeoJson JSON,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Create RegisteredMarkers table
CREATE TABLE IF NOT EXISTS RegisteredMarkers (
    MarkerId INT AUTO_INCREMENT PRIMARY KEY,
    Lat DOUBLE NOT NULL,
    Lng DOUBLE NOT NULL,
    Description VARCHAR(500),
    UserId INT,
    Organization VARCHAR(100),
    State ENUM('Unseen','Seen','Rejected','Accepted') DEFAULT 'Unseen',
    Type VARCHAR(100),
    HeightM DECIMAL(6,2),
    HeightMOverSea DECIMAL(6,2),
    AccuracyM DECIMAL(5,2),
    ObstacleCategory VARCHAR(50),
    IsTemporary BOOLEAN DEFAULT FALSE,
    Lighting VARCHAR(100),
    SubmittedBy INT,
    ReviewedBy INT NULL,
    ReviewComment VARCHAR(500),
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    Source VARCHAR(100),
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    GeoJson JSON,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    FOREIGN KEY (SubmittedBy) REFERENCES Users(UserId) ON DELETE SET NULL,
    FOREIGN KEY (ReviewedBy) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- Create ReviewAssign table
CREATE TABLE IF NOT EXISTS ReviewAssign (
    UserId INT,
    MarkerId INT,
    PRIMARY KEY (UserId, MarkerId),
    FOREIGN KEY (MarkerId) REFERENCES RegisteredMarkers(MarkerId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Create Notifications table
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    MarkerId INT NULL,
    Message VARCHAR(300) NOT NULL,
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsRead BOOLEAN DEFAULT FALSE,
    Type ENUM('Info', 'Warning', 'ReviewAssigned', 'MarkerAccepted', 'MarkerRejected') DEFAULT 'Info',
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Insert dummy data into Users table
INSERT INTO Users (UserId, Name, LastName, FirstName, UserType, Organization, PasswordHash, Email) VALUES
(1, 'Johnny', 'Test', 'J.', 'User', 'NLA', 'hash1', 'johnny@test.no'), -- Testbruker (Pilot 1)
(2, 'Kari', 'Hansen', 'K.', 'User', 'Luftforsvaret', 'hash2', 'kari@mil.no'), -- Pilot 2
(3, 'Per', 'Olsen', 'P.', 'User', 'NLA', 'hash3', 'per@nla.no'), -- Pilot 3
(4, 'Lars', 'Solberg', 'L.', 'User', 'Politiets helikoptertjeneste', 'hash4', 'lars@politi.no'), -- Pilot 4
(5, 'Svein', 'Admin', 'S.', 'Admin', 'Kartverket', 'hash5', 'svein@kart.no'), -- Administrator 1 (ReviewedBy)
(6, 'Anne', 'Monsen', 'A.', 'User', 'NLA', 'hash6', 'anne@nla.no'), -- Pilot 5 (FÅR > 5 rapporter)
(7, 'Ola', 'Nordmann', 'O.', 'User', 'Luftforsvaret', 'hash7', 'ola@mil.no'), -- Pilot 6
(8, 'Heidi', 'Larsen', 'H.', 'User', 'NLA', 'hash8', 'heidi@nla.no'), -- Pilot 7
(9, 'Jonas', 'Berg', 'J.', 'Employee', 'Kartverket', 'hash9', 'jonas@kart.no'), -- Administrator 2 (ReviewedBy)
(10, 'Eva', 'Jensen', 'E.', 'User', 'Politiets helikoptertjeneste', 'hash10', 'eva@politi.no'); -- Pilot 8

-- Delete existing data from Users table
DELETE FROM Users; 