
USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'KayArtDB')
BEGIN
    ALTER DATABASE KayArtDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE KayArtDB;
    PRINT 'Database KayArtDB dropped';
END
ELSE
BEGIN
    PRINT 'Database KayArtDB does not exist';
END
GO
