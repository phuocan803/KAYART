/* 04-view-all-data.sql */
/* View all data from all tables in KayArtDB */

USE KayArtDB;
GO

PRINT 'VIEWING ALL DATA FROM KAYARTDB';

-- Users
PRINT '1. Users Table:';
SELECT * FROM dbo.Users ORDER BY Id;
GO

-- OTP
PRINT '2. OTP Table:';
SELECT * FROM dbo.OTP ORDER BY Id;
GO

-- Projects
PRINT '3. Projects Table:';
SELECT * FROM dbo.Projects ORDER BY Id;
GO

-- SharedProjects
PRINT '4. SharedProjects Table:';
SELECT * FROM dbo.SharedProjects ORDER BY Id;
GO

-- Friends
PRINT '5. Friends Table:';
SELECT * FROM dbo.Friends ORDER BY Id;
GO

-- ChatMessages
PRINT '6. ChatMessages Table:';
SELECT * FROM dbo.ChatMessages ORDER BY Id;
GO

-- RoomChatMessages
PRINT '7. RoomChatMessages Table:';
SELECT * FROM dbo.RoomChatMessages ORDER BY Id;
GO

-- Rooms
PRINT '8. Rooms Table:';
SELECT * FROM dbo.Rooms ORDER BY Id;
GO

-- RoomCreationLog
PRINT '9. RoomCreationLog Table:';
SELECT * FROM dbo.RoomCreationLog ORDER BY Id;
GO

-- RoomMembers
PRINT '10. RoomMembers Table:';
SELECT * FROM dbo.RoomMembers ORDER BY Id;
GO

-- ProjectSessions
PRINT '11. ProjectSessions Table:';
SELECT * FROM dbo.ProjectSessions ORDER BY Id;
GO

-- RoomBitmaps
PRINT '12. RoomBitmaps Table:';
SELECT RoomCode, LEFT(BitmapData, 50) + '...' AS BitmapPreview, LastUpdated 
FROM dbo.RoomBitmaps 
ORDER BY RoomCode;
GO

PRINT '';
PRINT '=== End of data view ===';
GO
