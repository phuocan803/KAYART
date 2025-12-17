USE [KayArtDB];
GO

PRINT 'INSERTING TEST DATA';


DELETE FROM ProjectSessions;
DELETE FROM RoomBitmaps;
DELETE FROM RoomChatMessages;
DELETE FROM RoomMembers;
DELETE FROM RoomCreationLog;
DELETE FROM Rooms;
DELETE FROM ChatMessages;
DELETE FROM Friends;
DELETE FROM SharedProjects;
DELETE FROM Projects;
DELETE FROM OTP;
DELETE FROM Users;

-- Reset identity seeds
DBCC CHECKIDENT (Users, RESEED, 0);
DBCC CHECKIDENT (Projects, RESEED, 0);
DBCC CHECKIDENT (SharedProjects, RESEED, 0);
DBCC CHECKIDENT (Friends, RESEED, 0);
DBCC CHECKIDENT (ChatMessages, RESEED, 0);
DBCC CHECKIDENT (RoomChatMessages, RESEED, 0);
DBCC CHECKIDENT (Rooms, RESEED, 0);
DBCC CHECKIDENT (RoomCreationLog, RESEED, 0);
DBCC CHECKIDENT (RoomMembers, RESEED, 0);
DBCC CHECKIDENT (ProjectSessions, RESEED, 0);
GO


PRINT '1. Inserting 25 users...';

INSERT INTO Users (Username, PasswordHash, Email, FullName, PhoneNumber, IsOnline, CreatedAt)
VALUES
    -- English names (1-10) - Each with unique BCrypt hash for "123456"
    ('alice','$2a$11$SvuqlngKjkwCh/IwnNWvJuEaA5YIGNjuUabYR/14R7aPebdTDXUoW','alice@kayart.com','Alice Johnson','0901234567',1,DATEADD(DAY,-60,GETDATE())),
    ('bob','$2a$11$Pm/LOYfpWPoUThy8qufBpOkUnRbZpH/4essTv7i8ahQ6hfXgaD3k2','bob@kayart.com','Bob Smith','0907654321',1,DATEADD(DAY,-55,GETDATE())),
    ('charlie','$2a$11$/Jap/CwFgx3Bb70Hfhfmx.10kkdc1UALVo7tvdCscfNnwvMr4KFzK','charlie@kayart.com','Charlie Brown','0908888888',1,DATEADD(DAY,-50,GETDATE())),
    ('diana','$2a$11$KtbFcc2vjNiwyGQwUqUssuXVnyr.9ThhwU5hTXcaSEPcqU5CLrcuW','diana@kayart.com','Diana Prince','0909999999',0,DATEADD(DAY,-45,GETDATE())),
    ('emma','$2a$11$lgkiMLEX.dzbGeGli/a4x.LFCEw2amhe5IJdXMtyfMfew.wMc1WyG','emma@kayart.com','Emma Watson','0901111111',1,DATEADD(DAY,-40,GETDATE())),
    ('frank','$2a$11$8.BSn3kE9eUWSUH/pW4LjOy4wYieDHaCxhFNKOX5VdDxwmaggDyZm','frank@kayart.com','Frank Miller','0902222222',1,DATEADD(DAY,-35,GETDATE())),
    ('grace','$2a$11$tW1au7NrzC6tXTf0bqRBEO8v6PbHeYe7ntSBMNUpNmEjwnceO/08C','grace@kayart.com','Grace Lee','0903333333',1,DATEADD(DAY,-30,GETDATE())),
    ('henry','$2a$11$cyY3skR6hTmPsfXinltkvuUPVQpyYEtjI9sN3rdpL7QpAh/SMbUsG','henry@kayart.com','Henry Ford','0904444444',0,DATEADD(DAY,-25,GETDATE())),
    ('irene','$2a$11$BN5PTpYNdnTUfpHeV38vf.zSNSJCEfBNh4EAkqwNxcRdPI9Ov49rq','irene@kayart.com','Irene Chen','0905555555',0,DATEADD(DAY,-20,GETDATE())),
    ('jack','$2a$11$6wEJhKq1zsCqJOzJLHMCGeIGVebNspEkT6R.p081YkVYJyAqaQmpu','jack@kayart.com','Jack Wilson','0906666666',1,DATEADD(DAY,-15,GETDATE())),
    
    -- Vietnamese names (11-25) - Each with unique BCrypt hash for "123456"
    ('minh','$2a$11$hDOWGT4MMytabLBUeRhSMuB/QgIDTu7eKj/h2OlKY0NoAP5DJ1KQ2','minh@kayart.com',N'Nguyễn Văn Minh','0907777777',1,DATEADD(DAY,-48,GETDATE())),
    ('lan','$2a$11$niV3BwaMieTGXYqhhkG1yO3tgjJvbG9257jgHVweah4IqLIFPfCnu','lan@kayart.com',N'Trần Thị Lan','0908777777',1,DATEADD(DAY,-42,GETDATE())),
    ('huy','$2a$11$.rY4qSd/SPlRxmROGgkqa.DEf9.f7tS.ea1g2l76qpeUFjqLvmq46','huy@kayart.com',N'Lê Quang Huy','0909777777',0,DATEADD(DAY,-38,GETDATE())),
    ('thu','$2a$11$7lYVtXVJ/i2YbxkzuQDscOGRClIlAnQ8XZuwFBvtFzD06C1EVF6Mu','thu@kayart.com',N'Phạm Thu Thảo','0900777777',1,DATEADD(DAY,-33,GETDATE())),
    ('nam','$2a$11$JJ33nsmXd2oiko4P5CvxdO0BM6EfaOlkKMyggghVZoC.DFbGO6YOy','nam@kayart.com',N'Hoàng Văn Nam','0901777777',1,DATEADD(DAY,-28,GETDATE())),
    ('linh','$2a$11$simTLvbqM3msc3AwQibOc.JD1duR3z2jbiTqw5pSZfqyIobebKnjG','linh@kayart.com',N'Vũ Thị Linh','0902777777',0,DATEADD(DAY,-22,GETDATE())),
    ('tuan','$2a$11$nxAYHDw91uwOjIgvSpkLxOiIvZohSy51fF0G4WkPD8yWbjgP76O1q','tuan@kayart.com',N'Đặng Anh Tuấn','0903777777',1,DATEADD(DAY,-18,GETDATE())),
    ('ha','$2a$11$x2aHvBidX9MkpuBQMzbCkeBYvVfVLYcQKVdX8CKbQUrteCChk6u.a','ha@kayart.com',N'Bùi Thu Hà','0904777777',1,DATEADD(DAY,-14,GETDATE())),
    ('khoa','$2a$11$S8eQweqPSYMVL5izrcwh3.5Lt2pvcWIqPj6Y5PQ/75BelGGGdu7JG','khoa@kayart.com',N'Phan Minh Khoa','0905777777',0,DATEADD(DAY,-10,GETDATE())),
    ('nhi','$2a$11$hDmHmz/a7jx.sySpBvY1P.OOZoksT2r3uJAOEUA3EExBGNjWXb2s6','nhi@kayart.com',N'Võ Thị Nhi','0906777777',1,DATEADD(DAY,-8,GETDATE())),
    ('quan','$2a$11$TjRKU6Es.YAd3DJQXf1aA.OAnKLxr5KJBS8aq96azEk8BDdgdnh5e','quan@kayart.com',N'Trịnh Văn Quân','0907888888',1,DATEADD(DAY,-6,GETDATE())),
    ('phuong','$2a$11$SIWSoK6fq7oI1UKTmh6MF.H5AAoI7XxU54QVE.IRs6GiRElnrae2e','phuong@kayart.com',N'Lý Thị Phương','0908999999',1,DATEADD(DAY,-5,GETDATE())),
    ('dat','$2a$11$Gq.6AtMQErDk/xNoRrFM3Oo9jCQc2SbKBA335rTENRlZOTqMzoeqO','dat@kayart.com',N'Ngô Tiến Đạt','0909888888',0,DATEADD(DAY,-4,GETDATE())),
    ('vy','$2a$11$BgMOSeRd8YWGJ.RR1xuw3eF4UhCJcW0FVPucy/Ll6qtWfuys2gahu','vy@kayart.com',N'Đỗ Khánh Vy','0900888888',1,DATEADD(DAY,-3,GETDATE())),
    ('long','$2a$11$X9BQIix8BYvnlckoCQC2xOnJGbtb4y7aWixedu/dK.K/DwPScZUXS','long@kayart.com',N'Mai Hoàng Long','0901888888',1,DATEADD(DAY,-2,GETDATE()));
GO

PRINT '2. Inserting 50 projects...';

-- Sample base64 image (1x1 pixel PNG in different colors)
DECLARE @img1 NVARCHAR(MAX) = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';
DECLARE @img2 NVARCHAR(MAX) = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==';
DECLARE @img3 NVARCHAR(MAX) = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==';

-- Alice's projects (UserId=1) - 5 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (1, 'Mountain Landscape', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-40,GETDATE()), DATEADD(HOUR,-2,GETDATE())),
    (1, 'Abstract Geometry', @img2, @img2, 1024, 768, 1, 1234, DATEADD(DAY,-30,GETDATE()), DATEADD(MINUTE,-15,GETDATE())),
    (1, 'Sunset Beach', @img3, @img3, 1920, 1080, 0, NULL, DATEADD(DAY,-20,GETDATE()), DATEADD(HOUR,-5,GETDATE())),
    (1, 'City Night', @img1, @img1, 1280, 720, 0, NULL, DATEADD(DAY,-10,GETDATE()), DATEADD(DAY,-1,GETDATE())),
    (1, 'Forest Path', @img2, @img2, 1600, 900, 0, NULL, DATEADD(DAY,-5,GETDATE()), DATEADD(HOUR,-10,GETDATE()));

-- Bob's projects (UserId=2) - 4 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (2, 'City Skyline', @img1, @img1, 1280, 720, 0, NULL, DATEADD(DAY,-35,GETDATE()), DATEADD(DAY,-3,GETDATE())),
    (2, 'Portrait Study', @img2, @img2, 600, 800, 1, 5678, DATEADD(DAY,-25,GETDATE()), DATEADD(MINUTE,-30,GETDATE())),
    (2, 'Anime Character', @img3, @img3, 1024, 1024, 0, NULL, DATEADD(DAY,-15,GETDATE()), DATEADD(HOUR,-8,GETDATE())),
    (2, 'Watercolor Flowers', @img1, @img1, 800, 600, 0, NULL, DATEADD(DAY,-8,GETDATE()), DATEADD(HOUR,-4,GETDATE()));

-- Charlie's projects (UserId=3) - 4 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (3, 'Geometric Patterns', @img2, @img2, 1024, 1024, 0, NULL, DATEADD(DAY,-32,GETDATE()), DATEADD(DAY,-2,GETDATE())),
    (3, 'Nature Scene', @img3, @img3, 1600, 900, 1, 9012, DATEADD(DAY,-22,GETDATE()), DATEADD(MINUTE,-45,GETDATE())),
    (3, 'Digital Art', @img1, @img1, 2048, 1536, 0, NULL, DATEADD(DAY,-12,GETDATE()), DATEADD(HOUR,-6,GETDATE())),
    (3, 'Space Theme', @img2, @img2, 1920, 1080, 0, NULL, DATEADD(DAY,-6,GETDATE()), DATEADD(HOUR,-3,GETDATE()));

-- Diana's projects (UserId=4) - 3 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (4, 'Watercolor Landscape', @img3, @img3, 800, 600, 0, NULL, DATEADD(DAY,-28,GETDATE()), DATEADD(DAY,-4,GETDATE())),
    (4, 'Character Design', @img1, @img1, 1200, 1600, 0, NULL, DATEADD(DAY,-18,GETDATE()), DATEADD(DAY,-2,GETDATE())),
    (4, 'Fantasy Art', @img2, @img2, 1920, 1080, 0, NULL, DATEADD(DAY,-9,GETDATE()), DATEADD(HOUR,-7,GETDATE()));

-- Emma's projects (UserId=5) - 4 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (5, 'Space Exploration', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-26,GETDATE()), DATEADD(HOUR,-9,GETDATE())),
    (5, 'Fantasy Castle', @img2, @img2, 1440, 900, 1, 3456, DATEADD(DAY,-16,GETDATE()), DATEADD(MINUTE,-20,GETDATE())),
    (5, 'Cyberpunk City', @img3, @img3, 2560, 1440, 0, NULL, DATEADD(DAY,-11,GETDATE()), DATEADD(HOUR,-5,GETDATE())),
    (5, 'Ocean Depths', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-7,GETDATE()), DATEADD(HOUR,-2,GETDATE()));

-- Frank's projects (UserId=6) - 3 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (6, 'Minimalist Design', @img2, @img2, 1024, 768, 0, NULL, DATEADD(DAY,-24,GETDATE()), DATEADD(DAY,-1,GETDATE())),
    (6, 'Urban Photography', @img3, @img3, 1280, 960, 0, NULL, DATEADD(DAY,-14,GETDATE()), DATEADD(HOUR,-6,GETDATE())),
    (6, 'Street Art', @img1, @img1, 1600, 1200, 0, NULL, DATEADD(DAY,-4,GETDATE()), DATEADD(HOUR,-1,GETDATE()));

-- Grace's projects (UserId=7) - 3 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    (7, 'Anime Style', @img2, @img2, 1920, 1080, 0, NULL, DATEADD(DAY,-21,GETDATE()), DATEADD(DAY,-5,GETDATE())),
    (7, 'Manga Panel', @img3, @img3, 1024, 1448, 0, NULL, DATEADD(DAY,-13,GETDATE()), DATEADD(HOUR,-4,GETDATE())),
    (7, 'Chibi Characters', @img1, @img1, 800, 800, 0, NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(HOUR,-2,GETDATE()));

-- Vietnamese users' projects (11-25) - 24 projects
INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height, IsOnline, RoomID, CreatedAt, UpdatedAt)
VALUES
    -- Minh (11) - 3 projects
    (11, N'Phong Cảnh Việt Nam', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-27,GETDATE()), DATEADD(HOUR,-8,GETDATE())),
    (11, N'Chùa Một Cột', @img2, @img2, 1280, 720, 1, 7890, DATEADD(DAY,-17,GETDATE()), DATEADD(MINUTE,-10,GETDATE())),
    (11, N'Hội An Đêm', @img3, @img3, 1920, 1080, 0, NULL, DATEADD(DAY,-8,GETDATE()), DATEADD(HOUR,-3,GETDATE())),
    
    -- Lan (12) - 2 projects
    (12, N'Hoa Sen', @img1, @img1, 1024, 768, 0, NULL, DATEADD(DAY,-23,GETDATE()), DATEADD(DAY,-3,GETDATE())),
    (12, N'Áo Dài Truyền Thống', @img2, @img2, 800, 1200, 0, NULL, DATEADD(DAY,-12,GETDATE()), DATEADD(HOUR,-5,GETDATE())),
    
    -- Huy (13) - 2 projects
    (13, N'Sài Gòn Xưa', @img3, @img3, 1600, 900, 0, NULL, DATEADD(DAY,-19,GETDATE()), DATEADD(HOUR,-7,GETDATE())),
    (13, N'Phố Cổ Hà Nội', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-9,GETDATE()), DATEADD(HOUR,-4,GETDATE())),
    
    -- Thu (14) - 2 projects
    (14, N'Thiên Nhiên Việt', @img2, @img2, 1920, 1080, 1, 4567, DATEADD(DAY,-16,GETDATE()), DATEADD(MINUTE,-25,GETDATE())),
    (14, N'Vịnh Hạ Long', @img3, @img3, 2560, 1440, 0, NULL, DATEADD(DAY,-7,GETDATE()), DATEADD(HOUR,-2,GETDATE())),
    
    -- Nam (15) - 2 projects
    (15, N'Cầu Rồng Đà Nẵng', @img1, @img1, 1920, 1080, 0, NULL, DATEADD(DAY,-15,GETDATE()), DATEADD(HOUR,-6,GETDATE())),
    (15, N'Bà Nà Hills', @img2, @img2, 1600, 900, 0, NULL, DATEADD(DAY,-5,GETDATE()), DATEADD(HOUR,-1,GETDATE())),
    
    -- Linh (16) - 2 projects
    (16, N'Tranh Dân Gian', @img3, @img3, 1024, 1024, 0, NULL, DATEADD(DAY,-14,GETDATE()), DATEADD(HOUR,-5,GETDATE())),
    (16, N'Làng Nghề Việt', @img1, @img1, 1280, 720, 0, NULL, DATEADD(DAY,-4,GETDATE()), DATEADD(HOUR,-2,GETDATE())),
    
    -- Tuan (17) - 2 projects
    (17, 'Modern Architecture', @img2, @img2, 1920, 1080, 1, 8901, DATEADD(DAY,-13,GETDATE()), DATEADD(MINUTE,-35,GETDATE())),
    (17, 'Interior Design', @img3, @img3, 1600, 1200, 0, NULL, DATEADD(DAY,-6,GETDATE()), DATEADD(HOUR,-3,GETDATE())),
    
    -- Ha (18) - 2 projects
    (18, N'Hoa Đào Tết', @img1, @img1, 1024, 768, 0, NULL, DATEADD(DAY,-11,GETDATE()), DATEADD(HOUR,-4,GETDATE())),
    (18, N'Chợ Hoa Xuân', @img2, @img2, 1920, 1080, 0, NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(HOUR,-1,GETDATE())),
    
    -- Khoa (19) - 1 project
    (19, 'Tech Illustration', @img3, @img3, 1920, 1080, 0, NULL, DATEADD(DAY,-10,GETDATE()), DATEADD(HOUR,-5,GETDATE())),
    
    -- Nhi (20) - 1 project
    (20, N'Thời Trang Hiện Đại', @img1, @img1, 1080, 1920, 0, NULL, DATEADD(DAY,-8,GETDATE()), DATEADD(HOUR,-3,GETDATE())),
    
    -- Quan (21) - 1 project
    (21, 'Game Character Design', @img2, @img2, 1024, 1024, 0, NULL, DATEADD(DAY,-6,GETDATE()), DATEADD(HOUR,-2,GETDATE())),
    
    -- Phuong (22) - 1 project
    (22, N'Tranh Thủy Mặc', @img3, @img3, 1200, 900, 0, NULL, DATEADD(DAY,-5,GETDATE()), DATEADD(HOUR,-4,GETDATE())),
    
    -- Dat (23) - 1 project
    (23, 'Sci-Fi Concept', @img1, @img1, 2560, 1440, 0, NULL, DATEADD(DAY,-4,GETDATE()), DATEADD(HOUR,-2,GETDATE())),
    
    -- Vy (24) - 1 project
    (24, N'Nghệ Thuật Đương Đại', @img2, @img2, 1920, 1080, 0, NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(HOUR,-1,GETDATE())),
    
    -- Long (25) - 1 project
    (25, 'Digital Painting', @img3, @img3, 1920, 1080, 0, NULL, DATEADD(DAY,-2,GETDATE()), DATEADD(MINUTE,-30,GETDATE()));

PRINT '50 projects inserted';
PRINT '8 projects are currently online for collaboration';
GO

PRINT '3. Inserting shared projects...';

-- Alice shares with many friends
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (1, 2, 'view', DATEADD(DAY,-10,GETDATE())),    -- Mountain → Bob
    (1, 3, 'view', DATEADD(DAY,-9,GETDATE())),     -- Mountain → Charlie
    (1, 5, 'edit', DATEADD(DAY,-8,GETDATE())),     -- Mountain → Emma (edit)
    (2, 2, 'view', DATEADD(DAY,-7,GETDATE())),     -- Abstract → Bob
    (2, 11, 'edit', DATEADD(DAY,-6,GETDATE())),    -- Abstract → Minh (edit)
    (3, 3, 'view', DATEADD(DAY,-5,GETDATE())),     -- Sunset → Charlie
    (3, 6, 'view', DATEADD(DAY,-4,GETDATE())),     -- Sunset → Frank
    (4, 7, 'view', DATEADD(DAY,-3,GETDATE())),     -- City Night → Grace
    (5, 12, 'view', DATEADD(DAY,-2,GETDATE()));    -- Forest → Lan

-- Bob shares projects
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (6, 1, 'view', DATEADD(DAY,-12,GETDATE())),    -- Skyline → Alice
    (6, 3, 'view', DATEADD(DAY,-11,GETDATE())),    -- Skyline → Charlie
    (7, 1, 'edit', DATEADD(DAY,-10,GETDATE())),    -- Portrait → Alice (edit)
    (7, 4, 'view', DATEADD(DAY,-9,GETDATE())),     -- Portrait → Diana
    (8, 5, 'view', DATEADD(DAY,-8,GETDATE())),     -- Anime → Emma
    (9, 1, 'view', DATEADD(DAY,-7,GETDATE()));     -- Watercolor → Alice

-- Charlie shares projects
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (10, 1, 'view', DATEADD(DAY,-14,GETDATE())),   -- Geometric → Alice
    (10, 2, 'view', DATEADD(DAY,-13,GETDATE())),   -- Geometric → Bob
    (11, 5, 'edit', DATEADD(DAY,-12,GETDATE())),   -- Nature → Emma (edit)
    (11, 7, 'view', DATEADD(DAY,-11,GETDATE())),   -- Nature → Grace
    (12, 1, 'view', DATEADD(DAY,-10,GETDATE())),   -- Digital → Alice
    (13, 2, 'view', DATEADD(DAY,-9,GETDATE()));    -- Space → Bob

-- Diana shares
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (14, 1, 'view', DATEADD(DAY,-15,GETDATE())),   -- Watercolor → Alice
    (15, 2, 'view', DATEADD(DAY,-14,GETDATE())),   -- Character → Bob
    (16, 5, 'view', DATEADD(DAY,-13,GETDATE()));   -- Fantasy → Emma

-- Emma shares
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (17, 1, 'view', DATEADD(DAY,-8,GETDATE())),    -- Space → Alice
    (18, 6, 'edit', DATEADD(DAY,-7,GETDATE())),    -- Castle → Frank (edit)
    (19, 3, 'view', DATEADD(DAY,-6,GETDATE())),    -- Cyberpunk → Charlie
    (20, 11, 'view', DATEADD(DAY,-5,GETDATE()));   -- Ocean → Minh

-- Frank shares
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (21, 1, 'view', DATEADD(DAY,-6,GETDATE())),    -- Minimalist → Alice
    (22, 7, 'view', DATEADD(DAY,-5,GETDATE())),    -- Urban → Grace
    (23, 2, 'view', DATEADD(DAY,-4,GETDATE()));    -- Street → Bob

-- Grace shares
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (24, 3, 'view', DATEADD(DAY,-7,GETDATE())),    -- Anime → Charlie
    (25, 5, 'view', DATEADD(DAY,-6,GETDATE())),    -- Manga → Emma
    (26, 1, 'view', DATEADD(DAY,-5,GETDATE()));    -- Chibi → Alice

-- Vietnamese users share
INSERT INTO SharedProjects (ProjectId, UserId, Permission, SharedAt)
VALUES
    (27, 1, 'view', DATEADD(DAY,-9,GETDATE())),    -- Phong Cảnh → Alice
    (27, 12, 'edit', DATEADD(DAY,-8,GETDATE())),   -- Phong Cảnh → Lan (edit)
    (28, 2, 'view', DATEADD(DAY,-7,GETDATE())),    -- Chùa → Bob
    (30, 11, 'view', DATEADD(DAY,-6,GETDATE())),   -- Hoa Sen → Minh
    (32, 14, 'view', DATEADD(DAY,-5,GETDATE())),   -- Sài Gòn → Thu
    (34, 1, 'view', DATEADD(DAY,-4,GETDATE())),    -- Thiên Nhiên → Alice
    (36, 15, 'view', DATEADD(DAY,-3,GETDATE())),   -- Cầu Rồng → Nam
    (38, 16, 'view', DATEADD(DAY,-2,GETDATE())),   -- Tranh Dân Gian → Linh
    (40, 17, 'edit', DATEADD(DAY,-1,GETDATE()));   -- Modern → Tuan (edit)

PRINT '42 shared project relationships inserted';
GO


PRINT '4. Inserting friend relationships...';

-- Alice's friends (bidirectional accepted)
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (1, 2, 'accepted', DATEADD(DAY,-50,GETDATE())),  -- Alice ↔ Bob
    (2, 1, 'accepted', DATEADD(DAY,-50,GETDATE())),
    (1, 3, 'accepted', DATEADD(DAY,-45,GETDATE())),  -- Alice ↔ Charlie
    (3, 1, 'accepted', DATEADD(DAY,-45,GETDATE())),
    (1, 5, 'accepted', DATEADD(DAY,-40,GETDATE())),  -- Alice ↔ Emma
    (5, 1, 'accepted', DATEADD(DAY,-40,GETDATE())),
    (1, 6, 'accepted', DATEADD(DAY,-35,GETDATE())),  -- Alice ↔ Frank
    (6, 1, 'accepted', DATEADD(DAY,-35,GETDATE())),
    (1, 11, 'accepted', DATEADD(DAY,-30,GETDATE())), -- Alice ↔ Minh
    (11, 1, 'accepted', DATEADD(DAY,-30,GETDATE()));

-- Bob's additional friends
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (2, 3, 'accepted', DATEADD(DAY,-42,GETDATE())),  -- Bob ↔ Charlie
    (3, 2, 'accepted', DATEADD(DAY,-42,GETDATE())),
    (2, 4, 'accepted', DATEADD(DAY,-38,GETDATE())),  -- Bob ↔ Diana
    (4, 2, 'accepted', DATEADD(DAY,-38,GETDATE())),
    (2, 7, 'accepted', DATEADD(DAY,-32,GETDATE())),  -- Bob ↔ Grace
    (7, 2, 'accepted', DATEADD(DAY,-32,GETDATE()));

-- Charlie's additional friends
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (3, 5, 'accepted', DATEADD(DAY,-36,GETDATE())),  -- Charlie ↔ Emma
    (5, 3, 'accepted', DATEADD(DAY,-36,GETDATE())),
    (3, 7, 'accepted', DATEADD(DAY,-28,GETDATE())),  -- Charlie ↔ Grace
    (7, 3, 'accepted', DATEADD(DAY,-28,GETDATE())),
    (3, 12, 'accepted', DATEADD(DAY,-24,GETDATE())), -- Charlie ↔ Lan
    (12, 3, 'accepted', DATEADD(DAY,-24,GETDATE()));

-- Emma's additional friends
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (5, 6, 'accepted', DATEADD(DAY,-26,GETDATE())),  -- Emma ↔ Frank
    (6, 5, 'accepted', DATEADD(DAY,-26,GETDATE())),
    (5, 14, 'accepted', DATEADD(DAY,-22,GETDATE())), -- Emma ↔ Thu
    (14, 5, 'accepted', DATEADD(DAY,-22,GETDATE()));

-- Vietnamese users friendships
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (11, 12, 'accepted', DATEADD(DAY,-40,GETDATE())), -- Minh ↔ Lan
    (12, 11, 'accepted', DATEADD(DAY,-40,GETDATE())),
    (11, 14, 'accepted', DATEADD(DAY,-34,GETDATE())), -- Minh ↔ Thu
    (14, 11, 'accepted', DATEADD(DAY,-34,GETDATE())),
    (12, 14, 'accepted', DATEADD(DAY,-28,GETDATE())), -- Lan ↔ Thu
    (14, 12, 'accepted', DATEADD(DAY,-28,GETDATE())),
    (15, 17, 'accepted', DATEADD(DAY,-20,GETDATE())), -- Nam ↔ Tuan
    (17, 15, 'accepted', DATEADD(DAY,-20,GETDATE())),
    (18, 20, 'accepted', DATEADD(DAY,-16,GETDATE())), -- Ha ↔ Nhi
    (20, 18, 'accepted', DATEADD(DAY,-16,GETDATE())),
    (21, 22, 'accepted', DATEADD(DAY,-12,GETDATE())), -- Quan ↔ Phuong
    (22, 21, 'accepted', DATEADD(DAY,-12,GETDATE()));

-- Pending friend requests
INSERT INTO Friends (UserId, FriendId, Status, CreatedAt)
VALUES
    (1, 4, 'pending', DATEADD(DAY,-5,GETDATE())),    -- Alice → Diana (pending)
    (6, 7, 'pending', DATEADD(DAY,-4,GETDATE())),    -- Frank → Grace (pending)
    (10, 11, 'pending', DATEADD(DAY,-3,GETDATE())),  -- Jack → Minh (pending)
    (13, 15, 'pending', DATEADD(DAY,-2,GETDATE()));  -- Huy → Nam (pending)

PRINT '52 friend relationships inserted (48 accepted + 4 pending)';
GO

PRINT '5. Inserting private chat messages...';

-- Alice ↔ Bob conversation
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (1, 2, 'Hey Bob! How are you?', 1, DATEADD(HOUR,-24,GETDATE())),
    (2, 1, 'Hi Alice! I''m good, thanks!', 1, DATEADD(HOUR,-23,GETDATE())),
    (1, 2, 'Want to collaborate on a project?', 1, DATEADD(HOUR,-22,GETDATE())),
    (2, 1, 'Sure! Let''s do it!', 0, DATEADD(HOUR,-2,GETDATE()));

-- Alice ↔ Emma conversation
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (1, 5, 'Emma, check out my new landscape!', 1, DATEADD(HOUR,-18,GETDATE())),
    (5, 1, 'Wow, it looks amazing!', 1, DATEADD(HOUR,-17,GETDATE())),
    (5, 1, 'Can you share it with me?', 0, DATEADD(HOUR,-1,GETDATE()));

-- Bob ↔ Charlie conversation
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (2, 3, 'Charlie, are you free this weekend?', 1, DATEADD(HOUR,-12,GETDATE())),
    (3, 2, 'Yes! What''s up?', 1, DATEADD(HOUR,-11,GETDATE())),
    (2, 3, 'Let''s work on that geometric art together', 0, DATEADD(HOUR,-3,GETDATE()));

-- Minh ↔ Lan conversation (Vietnamese)
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (11, 12, N'Lan ơi, bạn có rảnh không?', 1, DATEADD(HOUR,-8,GETDATE())),
    (12, 11, N'Có chứ! Sao vậy Minh?', 1, DATEADD(HOUR,-7,GETDATE())),
    (11, 12, N'Mình muốn chia sẻ project phong cảnh với bạn', 0, DATEADD(HOUR,-4,GETDATE()));

-- Emma ↔ Frank conversation
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (5, 6, 'Frank, I need your feedback on my castle design', 1, DATEADD(HOUR,-6,GETDATE())),
    (6, 5, 'Let me take a look!', 1, DATEADD(HOUR,-5,GETDATE())),
    (6, 5, 'The architecture is stunning!', 0, DATEADD(MINUTE,-30,GETDATE()));

-- Thu ↔ Nam conversation (Vietnamese)
INSERT INTO ChatMessages (FromUserId, ToUserId, Message, IsRead, SentAt)
VALUES
    (14, 15, N'Nam ơi, bạn thấy tranh Vịnh Hạ Long của mình thế nào?', 1, DATEADD(HOUR,-4,GETDATE())),
    (15, 14, N'Đẹp lắm Thu! Màu sắc rất hài hòa', 1, DATEADD(HOUR,-3,GETDATE())),
    (14, 15, N'Cảm ơn bạn nhé!', 0, DATEADD(MINUTE,-45,GETDATE()));

PRINT '22 private chat messages inserted';
GO


PRINT '6. Inserting active rooms...';

INSERT INTO Rooms (RoomCode, OwnerId, OwnerName, IsActive, CreatedAt, LastActivity)
VALUES
    (1234, 1, 'alice', 1, DATEADD(HOUR,-3,GETDATE()), DATEADD(MINUTE,-5,GETDATE())),
    (5678, 2, 'bob', 1, DATEADD(HOUR,-4,GETDATE()), DATEADD(MINUTE,-10,GETDATE())),
    (9012, 3, 'charlie', 1, DATEADD(HOUR,-2,GETDATE()), DATEADD(MINUTE,-2,GETDATE())),
    (3456, 5, 'emma', 1, DATEADD(HOUR,-5,GETDATE()), DATEADD(MINUTE,-15,GETDATE())),
    (7890, 11, 'minh', 1, DATEADD(HOUR,-1,GETDATE()), DATEADD(MINUTE,-1,GETDATE())),
    (4567, 14, 'thu', 1, DATEADD(HOUR,-6,GETDATE()), DATEADD(MINUTE,-20,GETDATE())),
    (8901, 17, 'tuan', 1, DATEADD(MINUTE,-45,GETDATE()), DATEADD(MINUTE,-3,GETDATE())),
    (2345, 6, 'frank', 1, DATEADD(HOUR,-7,GETDATE()), DATEADD(MINUTE,-25,GETDATE())),
    (6789, 7, 'grace', 1, DATEADD(MINUTE,-90,GETDATE()), DATEADD(MINUTE,-8,GETDATE())),
    (1111, 12, 'lan', 1, DATEADD(HOUR,-8,GETDATE()), DATEADD(MINUTE,-30,GETDATE()));

-- Insert room creation log
INSERT INTO RoomCreationLog (UserId, RoomCode, CreatedAt)
VALUES
    (1, 1234, DATEADD(HOUR,-3,GETDATE())),
    (2, 5678, DATEADD(HOUR,-4,GETDATE())),
    (3, 9012, DATEADD(HOUR,-2,GETDATE())),
    (5, 3456, DATEADD(HOUR,-5,GETDATE())),
    (11, 7890, DATEADD(HOUR,-1,GETDATE())),
    (14, 4567, DATEADD(HOUR,-6,GETDATE())),
    (17, 8901, DATEADD(MINUTE,-45,GETDATE())),
    (6, 2345, DATEADD(HOUR,-7,GETDATE())),
    (7, 6789, DATEADD(MINUTE,-90,GETDATE())),
    (12, 1111, DATEADD(HOUR,-8,GETDATE()));

PRINT '10 active rooms inserted';
GO


PRINT '7. Inserting room members...';

-- Room 1234 (Alice's room) - 3 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (1234, 1, 'alice', DATEADD(HOUR,-3,GETDATE()), 1),
    (1234, 2, 'bob', DATEADD(HOUR,-2,GETDATE()), 1),
    (1234, 11, 'minh', DATEADD(HOUR,-1,GETDATE()), 1);

-- Room 5678 (Bob's room) - 2 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (5678, 2, 'bob', DATEADD(HOUR,-4,GETDATE()), 1),
    (5678, 4, 'diana', DATEADD(HOUR,-3,GETDATE()), 1);

-- Room 9012 (Charlie's room) - 4 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (9012, 3, 'charlie', DATEADD(HOUR,-2,GETDATE()), 1),
    (9012, 5, 'emma', DATEADD(MINUTE,-90,GETDATE()), 1),
    (9012, 7, 'grace', DATEADD(MINUTE,-60,GETDATE()), 1),
    (9012, 12, 'lan', DATEADD(MINUTE,-30,GETDATE()), 1);

-- Room 3456 (Emma's room) - 2 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (3456, 5, 'emma', DATEADD(HOUR,-5,GETDATE()), 1),
    (3456, 6, 'frank', DATEADD(HOUR,-4,GETDATE()), 1);

-- Room 7890 (Minh's room) - 3 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (7890, 11, 'minh', DATEADD(HOUR,-1,GETDATE()), 1),
    (7890, 1, 'alice', DATEADD(MINUTE,-50,GETDATE()), 1),
    (7890, 14, 'thu', DATEADD(MINUTE,-40,GETDATE()), 1);

-- Room 4567 (Thu's room) - 2 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (4567, 14, 'thu', DATEADD(HOUR,-6,GETDATE()), 1),
    (4567, 15, 'nam', DATEADD(HOUR,-5,GETDATE()), 1);

-- Room 8901 (Tuan's room) - 3 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (8901, 17, 'tuan', DATEADD(MINUTE,-45,GETDATE()), 1),
    (8901, 18, 'ha', DATEADD(MINUTE,-35,GETDATE()), 1),
    (8901, 20, 'nhi', DATEADD(MINUTE,-25,GETDATE()), 1);

-- Room 2345 (Frank's room) - 1 member
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (2345, 6, 'frank', DATEADD(HOUR,-7,GETDATE()), 1);

-- Room 6789 (Grace's room) - 2 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (6789, 7, 'grace', DATEADD(MINUTE,-90,GETDATE()), 1),
    (6789, 3, 'charlie', DATEADD(MINUTE,-80,GETDATE()), 1);

-- Room 1111 (Lan's room) - 2 members
INSERT INTO RoomMembers (RoomCode, UserId, Username, JoinedAt, IsCurrentlyInRoom)
VALUES
    (1111, 12, 'lan', DATEADD(HOUR,-8,GETDATE()), 1),
    (1111, 22, 'phuong', DATEADD(HOUR,-7,GETDATE()), 1);

PRINT '24 room members inserted across 10 rooms';
GO

PRINT '8. Inserting room chat messages...';

-- Room 1234 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (1234, 1, 'alice', 'Welcome to my room!', DATEADD(HOUR,-3,GETDATE())),
    (1234, 2, 'bob', 'Thanks Alice! Nice setup', DATEADD(HOUR,-2,GETDATE())),
    (1234, 11, 'minh', 'Hello everyone!', DATEADD(MINUTE,-90,GETDATE())),
    (1234, 1, 'alice', 'Let''s start drawing!', DATEADD(MINUTE,-60,GETDATE()));

-- Room 9012 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (9012, 3, 'charlie', 'Hey team!', DATEADD(HOUR,-2,GETDATE())),
    (9012, 5, 'emma', 'Hi Charlie!', DATEADD(MINUTE,-100,GETDATE())),
    (9012, 7, 'grace', 'Ready to collaborate', DATEADD(MINUTE,-70,GETDATE())),
    (9012, 12, 'lan', N'Chào mọi người!', DATEADD(MINUTE,-40,GETDATE())),
    (9012, 3, 'charlie', 'Let''s create something amazing', DATEADD(MINUTE,-20,GETDATE()));

-- Room 7890 messages (Vietnamese)
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (7890, 11, 'minh', N'Chào mừng đến phòng của mình!', DATEADD(HOUR,-1,GETDATE())),
    (7890, 1, 'alice', 'Thanks for inviting me!', DATEADD(MINUTE,-55,GETDATE())),
    (7890, 14, 'thu', N'Mình vẽ gì nhỉ?', DATEADD(MINUTE,-45,GETDATE())),
    (7890, 11, 'minh', N'Vẽ phong cảnh Việt Nam đi!', DATEADD(MINUTE,-35,GETDATE()));

-- Room 5678 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (5678, 2, 'bob', 'Working on portraits today', DATEADD(HOUR,-4,GETDATE())),
    (5678, 4, 'diana', 'Sounds good!', DATEADD(HOUR,-3,GETDATE())),
    (5678, 2, 'bob', 'Check out this technique', DATEADD(MINUTE,-120,GETDATE()));

-- Room 3456 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (3456, 5, 'emma', 'Frank, what do you think?', DATEADD(HOUR,-5,GETDATE())),
    (3456, 6, 'frank', 'Looking great Emma!', DATEADD(HOUR,-4,GETDATE())),
    (3456, 5, 'emma', 'Thanks! Let''s add more details', DATEADD(MINUTE,-180,GETDATE()));

-- Room 4567 messages (Vietnamese)
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (4567, 14, 'thu', N'Nam ơi, vẽ cùng mình nhé!', DATEADD(HOUR,-6,GETDATE())),
    (4567, 15, 'nam', N'Ok Thu! Vẽ gì đây?', DATEADD(HOUR,-5,GETDATE())),
    (4567, 14, 'thu', N'Vẽ thiên nhiên Việt Nam', DATEADD(MINUTE,-240,GETDATE()));

-- Room 8901 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (8901, 17, 'tuan', 'Modern architecture session!', DATEADD(MINUTE,-45,GETDATE())),
    (8901, 18, 'ha', N'Tuyệt vời!', DATEADD(MINUTE,-40,GETDATE())),
    (8901, 20, 'nhi', 'I love this style', DATEADD(MINUTE,-30,GETDATE())),
    (8901, 17, 'tuan', 'Let''s create something unique', DATEADD(MINUTE,-15,GETDATE()));

-- Room 6789 messages
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (6789, 7, 'grace', 'Anime drawing time!', DATEADD(MINUTE,-90,GETDATE())),
    (6789, 3, 'charlie', 'Yes! I love anime art', DATEADD(MINUTE,-85,GETDATE())),
    (6789, 7, 'grace', 'Let''s draw some characters', DATEADD(MINUTE,-70,GETDATE()));

-- Room 1111 messages (Vietnamese)
INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message, SentAt)
VALUES
    (1111, 12, 'lan', N'Chào Phương!', DATEADD(HOUR,-8,GETDATE())),
    (1111, 22, 'phuong', N'Chào Lan! Hôm nay vẽ gì?', DATEADD(HOUR,-7,GETDATE())),
    (1111, 12, 'lan', N'Vẽ hoa sen nhé!', DATEADD(MINUTE,-360,GETDATE()));

PRINT '34 room chat messages inserted';
GO


PRINT '9. Inserting project collaboration sessions...';

INSERT INTO ProjectSessions (ProjectId, UserId, JoinedAt, LastActivity, IsActive)
VALUES
    -- Project 2 (Abstract Geometry - Room 1234) - Alice's project
    (2, 1, DATEADD(HOUR,-3,GETDATE()), DATEADD(MINUTE,-5,GETDATE()), 1),
    (2, 2, DATEADD(HOUR,-2,GETDATE()), DATEADD(MINUTE,-10,GETDATE()), 1),
    (2, 11, DATEADD(HOUR,-1,GETDATE()), DATEADD(MINUTE,-15,GETDATE()), 1),
    
    -- Project 7 (Portrait Study - Room 5678) - Bob's project
    (7, 2, DATEADD(HOUR,-4,GETDATE()), DATEADD(MINUTE,-8,GETDATE()), 1),
    (7, 1, DATEADD(HOUR,-3,GETDATE()), DATEADD(MINUTE,-12,GETDATE()), 1),
    
    -- Project 11 (Nature Scene - Room 9012) - Charlie's project
    (11, 3, DATEADD(HOUR,-2,GETDATE()), DATEADD(MINUTE,-3,GETDATE()), 1),
    (11, 5, DATEADD(MINUTE,-90,GETDATE()), DATEADD(MINUTE,-5,GETDATE()), 1),
    (11, 7, DATEADD(MINUTE,-60,GETDATE()), DATEADD(MINUTE,-7,GETDATE()), 1),
    
    -- Project 18 (Fantasy Castle - Room 3456) - Emma's project
    (18, 5, DATEADD(HOUR,-5,GETDATE()), DATEADD(MINUTE,-18,GETDATE()), 1),
    (18, 6, DATEADD(HOUR,-4,GETDATE()), DATEADD(MINUTE,-20,GETDATE()), 1),
    
    -- Project 28 (Chùa Một Cột - Room 7890) - Minh's project
    (28, 11, DATEADD(HOUR,-1,GETDATE()), DATEADD(MINUTE,-2,GETDATE()), 1),
    (28, 1, DATEADD(MINUTE,-50,GETDATE()), DATEADD(MINUTE,-4,GETDATE()), 1),
    
    -- Project 34 (Thiên Nhiên Việt - Room 4567) - Thu's project
    (34, 14, DATEADD(HOUR,-6,GETDATE()), DATEADD(MINUTE,-22,GETDATE()), 1),
    (34, 15, DATEADD(HOUR,-5,GETDATE()), DATEADD(MINUTE,-25,GETDATE()), 1),
    
    -- Project 40 (Modern Architecture - Room 8901) - Tuan's project
    (40, 17, DATEADD(MINUTE,-45,GETDATE()), DATEADD(MINUTE,-6,GETDATE()), 1),
    (40, 18, DATEADD(MINUTE,-35,GETDATE()), DATEADD(MINUTE,-8,GETDATE()), 1);

PRINT '16 project collaboration sessions inserted';
GO

PRINT '10. Inserting room bitmaps...';

-- Sample bitmap data (base64 encoded small PNG)
DECLARE @bitmapData NVARCHAR(MAX) = 'iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAFUlEQVR42mNk+M9Qz0AEYBxVSF+FAP0QBgFPnHQWAAAAAElFTkSuQmCC';

INSERT INTO RoomBitmaps (RoomCode, BitmapData, LastUpdated)
VALUES
    (1234, @bitmapData, DATEADD(MINUTE,-5,GETDATE())),
    (5678, @bitmapData, DATEADD(MINUTE,-10,GETDATE())),
    (9012, @bitmapData, DATEADD(MINUTE,-2,GETDATE())),
    (7890, @bitmapData, DATEADD(MINUTE,-1,GETDATE())),
    (4567, @bitmapData, DATEADD(MINUTE,-20,GETDATE())),
    (8901, @bitmapData, DATEADD(MINUTE,-3,GETDATE()));

PRINT '6 room bitmaps inserted (persistent canvas data)';
GO
