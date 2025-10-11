USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KayArtDB')
BEGIN
    CREATE DATABASE KayArtDB;
    PRINT 'Database KayArtDB created';
END
GO

USE KayArtDB;
GO

PRINT '1. Creating Users table...';
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    FullName NVARCHAR(255),
    PhoneNumber NVARCHAR(50),
    AvatarBase64 NVARCHAR(MAX),
    IsOnline BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_IsOnline ON Users(IsOnline);
PRINT 'Table Users created';
GO


PRINT '2. Creating OTP table...';
CREATE TABLE OTP (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    OTPCode NVARCHAR(10) NOT NULL,
    ExpiryTime DATETIME2 NOT NULL,
    IsUsed BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
CREATE INDEX IX_OTP_Email ON OTP(Email);
PRINT 'Table OTP created';
GO


PRINT '3. Creating Projects table...';
CREATE TABLE Projects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OwnerId INT NOT NULL,
    ProjectName NVARCHAR(255) NOT NULL,
    ImageData NVARCHAR(MAX),
    ThumbnailData NVARCHAR(MAX),
    Width INT,
    Height INT,
    IsOnline BIT DEFAULT 0,              
    RoomID INT NULL,                     
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IX_Projects_OwnerId ON Projects(OwnerId);
CREATE INDEX IX_Projects_UpdatedAt ON Projects(UpdatedAt DESC);
CREATE INDEX IX_Projects_IsOnline ON Projects(IsOnline, UpdatedAt DESC);
CREATE INDEX IX_Projects_RoomID ON Projects(RoomID) WHERE RoomID IS NOT NULL;
PRINT 'Table Projects created with collaboration support';
GO


PRINT '4. Creating SharedProjects table...';
CREATE TABLE SharedProjects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    UserId INT NOT NULL,
    Permission NVARCHAR(20) DEFAULT 'view',
    SharedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE(ProjectId, UserId)
);
CREATE INDEX IX_SharedProjects_UserId ON SharedProjects(UserId);
PRINT 'Table SharedProjects created';
GO

PRINT '5. Creating Friends table...';
CREATE TABLE Friends (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    FriendId INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'pending',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (FriendId) REFERENCES Users(Id),
    UNIQUE(UserId, FriendId)
);
CREATE INDEX IX_Friends_UserId ON Friends(UserId, Status);
CREATE INDEX IX_Friends_FriendId ON Friends(FriendId, Status);
PRINT 'Table Friends created';
GO

PRINT '6. Creating ChatMessages table...';
CREATE TABLE ChatMessages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FromUserId INT NOT NULL,
    ToUserId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    SentAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (FromUserId) REFERENCES Users(Id),
    FOREIGN KEY (ToUserId) REFERENCES Users(Id)
);
CREATE INDEX IX_ChatMessages_From ON ChatMessages(FromUserId, SentAt DESC);
CREATE INDEX IX_ChatMessages_To ON ChatMessages(ToUserId, IsRead, SentAt DESC);
CREATE INDEX IX_ChatMessages_Conversation ON ChatMessages(FromUserId, ToUserId);
PRINT 'Table ChatMessages created';
GO


PRINT '7. Creating RoomChatMessages table...';
CREATE TABLE RoomChatMessages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomCode INT NOT NULL,
    UserId INT NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
CREATE INDEX IX_RoomChatMessages_RoomCode ON RoomChatMessages(RoomCode);
PRINT 'Table RoomChatMessages created';
GO

PRINT '8. Creating Rooms table...';
CREATE TABLE Rooms (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomCode INT NOT NULL UNIQUE,
    OwnerId INT NOT NULL,
    OwnerName NVARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    LastActivity DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (OwnerId) REFERENCES Users(Id)
);
CREATE INDEX IX_Rooms_RoomCode ON Rooms(RoomCode);
CREATE INDEX IX_Rooms_IsActive ON Rooms(IsActive, LastActivity DESC);
PRINT 'Table Rooms created';
GO


PRINT '9. Creating RoomCreationLog table...';
CREATE TABLE RoomCreationLog (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RoomCode INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
CREATE INDEX IX_RoomLog_UserId ON RoomCreationLog(UserId, CreatedAt DESC);
PRINT 'Table RoomCreationLog created';
GO


PRINT '10. Creating RoomMembers table...';
CREATE TABLE RoomMembers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomCode INT NOT NULL,
    UserId INT NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    JoinedAt DATETIME2 DEFAULT GETDATE(),
    LeftAt DATETIME2 NULL,
    IsCurrentlyInRoom BIT DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE(RoomCode, UserId)
);
CREATE INDEX IX_RoomMembers_RoomCode ON RoomMembers(RoomCode);
CREATE INDEX IX_RoomMembers_IsCurrently ON RoomMembers(IsCurrentlyInRoom);
PRINT 'Table RoomMembers created';
GO


PRINT '11. Creating ProjectSessions table...';
CREATE TABLE ProjectSessions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT NOT NULL,
    UserId INT NOT NULL,
    JoinedAt DATETIME2 DEFAULT GETDATE(),
    LastActivity DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE(ProjectId, UserId)
);
CREATE INDEX IX_ProjectSessions_ProjectId ON ProjectSessions(ProjectId, IsActive, LastActivity DESC);
CREATE INDEX IX_ProjectSessions_UserId ON ProjectSessions(UserId, IsActive);
PRINT 'Table ProjectSessions created';
GO


PRINT '12. Creating RoomBitmaps table...';
CREATE TABLE RoomBitmaps (
    RoomCode INT PRIMARY KEY,
    BitmapData NVARCHAR(MAX) NOT NULL,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RoomBitmaps_Rooms FOREIGN KEY (RoomCode) 
        REFERENCES Rooms(RoomCode) ON DELETE CASCADE
);
CREATE INDEX IX_RoomBitmaps_LastUpdated ON RoomBitmaps(LastUpdated);
PRINT 'Table RoomBitmaps created';
GO
