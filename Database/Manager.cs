using System;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BCrypt.Net; 
namespace Database
{
    public class Manager : IDisposable
    {
        private readonly string connectionString;
        private static readonly object lockObject = new object();


        public Manager()
        {
            try
            {
                connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Connection string is empty!");
                }

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        public Manager(string customConnectionString)
        {
            connectionString = customConnectionString;
            InitializeDatabase();
        }



        private void InitializeDatabase()
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        
                    }
                }
                catch (SqlException sqlEx)
                {
                    string errorMsg = $"Database connection failed!\n\n";
                    errorMsg += $"Error: {sqlEx.Message}\n";
                    errorMsg += $"Error Number: {sqlEx.Number}\n\n";

                    switch (sqlEx.Number)
                    {
                        case 18456:
                            errorMsg += "Possible causes:\n- Incorrect username or password\n- User doesn't have permission\n- SQL Server authentication not enabled\n";
                            break;
                        case 53:
                        case -1:
                            errorMsg += "Possible causes:\n- SQL Server is not running\n- Incorrect server name or port\n- Firewall blocking connection\n";
                            break;
                        case 4060:
                            errorMsg += "Database doesn't exist or user doesn't have access.\n";
                            errorMsg += "\nRUN THESE SQL SCRIPTS FIRST:\n";
                            errorMsg += "1. Database/01-drop-database.sql\n";
                            errorMsg += "2. Database/02-create-database.sql\n";
                            errorMsg += "3. Database/03-insert-data.sql\n";
                            break;
                    }

                    throw new Exception(errorMsg, sqlEx);
                }
            }
        }

        private void CreateTables(SqlConnection conn)
        {
            string[] tables = new[]
            {
                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]'))
                  CREATE TABLE [dbo].[Users] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [Username] NVARCHAR(100) UNIQUE NOT NULL,
                      [PasswordHash] NVARCHAR(255) NOT NULL,
                      [Email] NVARCHAR(255) UNIQUE NOT NULL,
                      [FullName] NVARCHAR(255),
                      [PhoneNumber] NVARCHAR(20),
                      [AvatarBase64] NVARCHAR(MAX),
                      [IsOnline] BIT DEFAULT 0,
                      [CreatedAt] DATETIME2 DEFAULT GETDATE()
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OTP]'))
                  CREATE TABLE [dbo].[OTP] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [Email] NVARCHAR(255) NOT NULL,
                      [OTPCode] NVARCHAR(10) NOT NULL,
                      [ExpiryTime] DATETIME2 NOT NULL,
                      [IsUsed] BIT DEFAULT 0,
                      [CreatedAt] DATETIME2 DEFAULT GETDATE()
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]'))
                  CREATE TABLE [dbo].[Projects] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [OwnerId] INT NOT NULL,
                      [ProjectName] NVARCHAR(255) NOT NULL,
                      [ImageData] NVARCHAR(MAX),
                      [ThumbnailData] NVARCHAR(MAX),
                      [Width] INT,
                      [Height] INT,
                      [CreatedAt] DATETIME2 DEFAULT GETDATE(),
                      [UpdatedAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([OwnerId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SharedProjects]'))
                  CREATE TABLE [dbo].[SharedProjects] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [ProjectId] INT NOT NULL,
                      [UserId] INT NOT NULL,
                      [Permission] NVARCHAR(20) DEFAULT 'view',
                      [SharedAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([Id]) ON DELETE CASCADE,
                      FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Friends]'))
                  CREATE TABLE [dbo].[Friends] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [UserId] INT NOT NULL,
                      [FriendId] INT NOT NULL,
                      [Status] NVARCHAR(20) DEFAULT 'pending',
                      [CreatedAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
                      FOREIGN KEY ([FriendId]) REFERENCES [Users]([Id]),
                      CONSTRAINT UQ_Friends UNIQUE (UserId, FriendId)
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChatMessages]'))
                  CREATE TABLE [dbo].[ChatMessages] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [FromUserId] INT NOT NULL,
                      [ToUserId] INT NOT NULL,
                      [Message] NVARCHAR(MAX) NOT NULL,
                      [IsRead] BIT DEFAULT 0,
                      [SentAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([FromUserId]) REFERENCES [Users]([Id]),
                      FOREIGN KEY ([ToUserId]) REFERENCES [Users]([Id])
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChatMessages]'))
                  CREATE TABLE [dbo].[RoomChatMessages] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [RoomCode] INT NOT NULL,
                      [UserId] INT NOT NULL,
                      [Username] NVARCHAR(100) NOT NULL,
                      [Message] NVARCHAR(MAX) NOT NULL,
                      [SentAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rooms]'))
                  CREATE TABLE [dbo].[Rooms] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [RoomCode] INT UNIQUE NOT NULL,
                      [OwnerId] INT NOT NULL,
                      [OwnerName] NVARCHAR(100) NOT NULL,
                      [IsActive] BIT DEFAULT 1,
                      [CreatedAt] DATETIME2 DEFAULT GETDATE(),
                      [LastActivity] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([OwnerId]) REFERENCES [Users]([Id])
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomCreationLog]'))
                  CREATE TABLE [dbo].[RoomCreationLog] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [UserId] INT NOT NULL,
                      [RoomCode] INT NOT NULL,
                      [CreatedAt] DATETIME2 DEFAULT GETDATE(),
                      FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
                  )",

                @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomMembers]'))
                  CREATE TABLE [dbo].[RoomMembers] (
                      [Id] INT PRIMARY KEY IDENTITY(1,1),
                      [RoomCode] INT NOT NULL,
                      [UserId] INT NOT NULL,
                      [Username] NVARCHAR(100) NOT NULL,
                      [JoinedAt] DATETIME2 DEFAULT GETDATE(),
                      [LeftAt] DATETIME2 NULL,
                      [IsCurrentlyInRoom] BIT DEFAULT 1,
                      FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
                      CONSTRAINT UQ_RoomMembers UNIQUE (RoomCode, UserId)
                  )"
            };

            foreach (var sql in tables)
                ExecuteNonQuery(conn, sql);
        }

        private void CreateIndexes(SqlConnection conn)
        {
            string[] indexes = new[]
            {
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email') CREATE INDEX IX_Users_Email ON Users(Email)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username') CREATE INDEX IX_Users_Username ON Users(Username)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OTP_Email') CREATE INDEX IX_OTP_Email ON OTP(Email)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_OwnerId') CREATE INDEX IX_Projects_OwnerId ON Projects(OwnerId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_UpdatedAt') CREATE INDEX IX_Projects_UpdatedAt ON Projects(UpdatedAt DESC)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SharedProjects_UserId') CREATE INDEX IX_SharedProjects_UserId ON SharedProjects(UserId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SharedProjects_ProjectId') CREATE INDEX IX_SharedProjects_ProjectId ON SharedProjects(ProjectId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Friends_UserId') CREATE INDEX IX_Friends_UserId ON Friends(UserId, Status)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Friends_FriendId') CREATE INDEX IX_Friends_FriendId ON Friends(FriendId, Status)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chat_From') CREATE INDEX IX_Chat_From ON ChatMessages(FromUserId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chat_To') CREATE INDEX IX_Chat_To ON ChatMessages(ToUserId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoomChat_RoomCode') CREATE INDEX IX_RoomChat_RoomCode ON RoomChatMessages(RoomCode)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoomChat_UserId') CREATE INDEX IX_RoomChat_UserId ON RoomChatMessages(UserId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoomMembers_RoomCode') CREATE INDEX IX_RoomMembers_RoomCode ON RoomMembers(RoomCode)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoomMembers_UserId') CREATE INDEX IX_RoomMembers_UserId ON RoomMembers(UserId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoomMembers_IsCurrently') CREATE INDEX IX_RoomMembers_IsCurrently ON RoomMembers(IsCurrentlyInRoom)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rooms_OwnerId') CREATE INDEX IX_Rooms_OwnerId ON Rooms(OwnerId)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rooms_RoomCode') CREATE INDEX IX_Rooms_RoomCode ON Rooms(RoomCode)",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rooms_IsActive') CREATE INDEX IX_Rooms_IsActive ON Rooms(IsActive, LastActivity DESC)"
            };

            foreach (var sql in indexes)
                ExecuteNonQuery(conn, sql);
        }

        private void ExecuteNonQuery(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandTimeout = 60;
                cmd.ExecuteNonQuery();
            }
        }



        private string HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Password cannot be empty");
                }
                
                string hash = BCrypt.Net.BCrypt.HashPassword(password, 11);
                
                
                return hash;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    return false;
                }
                
                if (string.IsNullOrEmpty(hash))
                {
                    return false;
                }
                
                if (!hash.StartsWith("$2a$") && !hash.StartsWith("$2b$") && !hash.StartsWith("$2y$"))
                {
                    return false;
                }
                
                
                bool result = BCrypt.Net.BCrypt.Verify(password, hash);
                
                
                return result;
            }
            catch (BCrypt.Net.SaltParseException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public int GetUserIdByUsername(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Id FROM Users WHERE Username = @username", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateUserOnlineStatus(int userId, bool isOnline)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Users SET IsOnline = @status WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@status", isOnline);
                            cmd.Parameters.AddWithValue("@id", userId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public string GetUserAvatar(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT AvatarBase64 FROM Users WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateUserAvatar(int userId, string avatarBase64)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Users SET AvatarBase64 = @avatar WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@avatar", avatarBase64 ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", userId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool LoginUser(string username, string password)
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    using (SqlCommand cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Username = @user", conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        
                        object result = cmd.ExecuteScalar();
                        
                        if (result == null || result == DBNull.Value)
                        {
                            return false;
                        }
                        
                        string storedHash = result.ToString();
                        
                        if (string.IsNullOrEmpty(storedHash))
                        {
                            return false;
                        }
                        
                        
                        bool isValid = VerifyPassword(password, storedHash);
                        
                        
                        return isValid;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RegisterUser(string username, string password, string email, string fullName, string phoneNumber)
        {
            lock (lockObject)
            {
                try
                {
                    
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        string hashedPassword = HashPassword(password);
                        
                        
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Users (Username, PasswordHash, Email, FullName, PhoneNumber) VALUES (@user, @pass, @email, @name, @phone)", conn))
                        {
                            cmd.Parameters.AddWithValue("@user", username);
                            cmd.Parameters.AddWithValue("@pass", hashedPassword);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@name", fullName ?? "");
                            cmd.Parameters.AddWithValue("@phone", phoneNumber ?? "");
                            
                            int rowsAffected = cmd.ExecuteNonQuery();
                            
                            
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                    {
                    }
                    else
                    {
;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public List<UserInfo> SearchUsers(string searchTerm, int currentUserId, int limit = 10)
        {
            List<UserInfo> users = new List<UserInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT TOP (@limit) Id, Username, Email, AvatarBase64, IsOnline FROM Users WHERE (Username LIKE @search OR Email LIKE @search) AND Id != @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                        cmd.Parameters.AddWithValue("@uid", currentUserId);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserInfo
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    AvatarBase64 = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    IsOnline = reader.GetBoolean(4)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return users;
        }



        public List<Project> GetUserProjects(int userId)
        {
            List<Project> projects = new List<Project>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Id, ProjectName, ThumbnailData, UpdatedAt FROM Projects WHERE OwnerId = @uid ORDER BY UpdatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projects.Add(new Project
                                {
                                    Id = reader.GetInt32(0),
                                    ProjectName = reader.GetString(1),
                                    ThumbnailData = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    UpdatedAt = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return projects;
        }

        public List<SharedProject> GetSharedProjects(int userId)
        {
            List<SharedProject> projects = new List<SharedProject>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT p.Id, p.ProjectName, u.Username, sp.Permission, p.ThumbnailData, sp.SharedAt
                          FROM SharedProjects sp
                          JOIN Projects p ON sp.ProjectId = p.Id
                          JOIN Users u ON p.OwnerId = u.Id
                          WHERE sp.UserId = @uid
                          ORDER BY sp.SharedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projects.Add(new SharedProject
                                {
                                    ProjectId = reader.GetInt32(0),
                                    ProjectName = reader.GetString(1),
                                    OwnerUsername = reader.GetString(2),
                                    Permission = reader.GetString(3),
                                    ThumbnailData = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    SharedAt = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return projects;
        }

        public Project GetProjectById(int projectId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Id, ProjectName, ImageData, ThumbnailData, CreatedAt, UpdatedAt FROM Projects WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", projectId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Project
                                {
                                    Id = reader.GetInt32(0),
                                    ProjectName = reader.GetString(1),
                                    ImageData = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    ThumbnailData = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    CreatedAt = reader.GetDateTime(4),
                                    UpdatedAt = reader.GetDateTime(5)
                                };
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public int CreateProject(int userId, string projectName, string imageData, string thumbnailData, int width, int height)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Projects (OwnerId, ProjectName, ImageData, ThumbnailData, Width, Height) VALUES (@uid, @name, @img, @thumb, @w, @h); SELECT SCOPE_IDENTITY();", conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@name", projectName);
                            cmd.Parameters.AddWithValue("@img", imageData ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@thumb", thumbnailData ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@w", width);
                            cmd.Parameters.AddWithValue("@h", height);
                            return Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        public bool UpdateProject(int projectId, string projectName, string description)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Projects SET ProjectName = @name, UpdatedAt = GETDATE() WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@name", projectName);
                            cmd.Parameters.AddWithValue("@id", projectId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool UpdateProjectImage(int projectId, string imageData, string thumbnailData)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Projects SET ImageData = @img, ThumbnailData = @thumb, UpdatedAt = GETDATE() WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@img", imageData ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@thumb", thumbnailData ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", projectId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool DeleteProject(int projectId)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Projects WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", projectId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool ShareProject(int projectId, int sharedWithUserId, string permission = "view")
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        using (SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM SharedProjects WHERE ProjectId = @pid AND UserId = @uid", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@pid", projectId);
                            checkCmd.Parameters.AddWithValue("@uid", sharedWithUserId);
                            
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                using (SqlCommand updateCmd = new SqlCommand(
                                    "UPDATE SharedProjects SET Permission = @perm WHERE ProjectId = @pid AND UserId = @uid", conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@perm", permission);
                                    updateCmd.Parameters.AddWithValue("@pid", projectId);
                                    updateCmd.Parameters.AddWithValue("@uid", sharedWithUserId);
                                    return updateCmd.ExecuteNonQuery() > 0;
                                }
                            }
                        }
                        
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO SharedProjects (ProjectId, UserId, Permission) VALUES (@pid, @uid, @perm)", conn))
                        {
                            cmd.Parameters.AddWithValue("@pid", projectId);
                            cmd.Parameters.AddWithValue("@uid", sharedWithUserId);
                            cmd.Parameters.AddWithValue("@perm", permission);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool UnshareProject(int projectId, int sharedWithUserId)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "DELETE FROM SharedProjects WHERE ProjectId = @pid AND UserId = @uid", conn))
                        {
                            cmd.Parameters.AddWithValue("@pid", projectId);
                            cmd.Parameters.AddWithValue("@uid", sharedWithUserId);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<SharedUserInfo> GetProjectShares(int projectId)
        {
            List<SharedUserInfo> shares = new List<SharedUserInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT u.Id, u.Username, u.AvatarBase64, sp.Permission, sp.SharedAt
                          FROM SharedProjects sp
                          JOIN Users u ON sp.UserId = u.Id
                          WHERE sp.ProjectId = @pid
                          ORDER BY sp.SharedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", projectId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shares.Add(new SharedUserInfo
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    AvatarBase64 = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Permission = reader.GetString(3),
                                    SharedAt = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return shares;
        }



        public bool SendFriendRequest(int userId, int friendUserId)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        using (SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM Friends WHERE (UserId = @uid AND FriendId = @fid) OR (UserId = @fid AND FriendId = @uid)", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@uid", userId);
                            checkCmd.Parameters.AddWithValue("@fid", friendUserId);
                            
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                return true;
                            }
                        }
                        
                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "INSERT INTO Friends (UserId, FriendId, Status) VALUES (@uid, @fid, 'accepted')", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@uid", userId);
                                    cmd.Parameters.AddWithValue("@fid", friendUserId);
                                    cmd.ExecuteNonQuery();
                                }

                                using (SqlCommand cmd = new SqlCommand(
                                    "INSERT INTO Friends (UserId, FriendId, Status) VALUES (@fid, @uid, 'accepted')", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@uid", userId);
                                    cmd.Parameters.AddWithValue("@fid", friendUserId);
                                    cmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                return true;
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<FriendInfo> GetFriendList(int userId)
        {
            List<FriendInfo> friends = new List<FriendInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT u.Id, u.Username, u.AvatarBase64, u.IsOnline
                          FROM Friends f
                          JOIN Users u ON f.FriendId = u.Id
                          WHERE f.UserId = @uid AND f.Status = 'accepted'
                          ORDER BY u.IsOnline DESC, u.Username", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                friends.Add(new FriendInfo
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    AvatarBase64 = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    IsOnline = reader.GetBoolean(3)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return friends;
        }



        public bool SendChatMessage(int fromUserId, int toUserId, string message)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO ChatMessages (FromUserId, ToUserId, Message) VALUES (@from, @to, @msg)", conn))
                        {
                            cmd.Parameters.AddWithValue("@from", fromUserId);
                            cmd.Parameters.AddWithValue("@to", toUserId);
                            cmd.Parameters.AddWithValue("@msg", message);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<ChatMessage> GetChatMessages(int userId, int friendId, int limit = 50)
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP (@limit) c.FromUserId, u.Username, c.Message, c.SentAt
                          FROM ChatMessages c
                          JOIN Users u ON c.FromUserId = u.Id
                          WHERE (c.FromUserId = @uid AND c.ToUserId = @fid) OR (c.FromUserId = @fid AND c.ToUserId = @uid)
                          ORDER BY c.SentAt", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@fid", friendId);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(new ChatMessage
                                {
                                    FromUserId = reader.GetInt32(0),
                                    FromUsername = reader.GetString(1),
                                    Message = reader.GetString(2),
                                    SentAt = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return messages;
        }

        public bool MarkMessagesAsRead(int fromUserId, int toUserId)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE ChatMessages SET IsRead = 1 WHERE FromUserId = @from AND ToUserId = @to AND IsRead = 0", conn))
                        {
                            cmd.Parameters.AddWithValue("@from", fromUserId);
                            cmd.Parameters.AddWithValue("@to", toUserId);
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }



        public bool CheckEmailExists(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @email", conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public string GenerateAndSaveOTP(string email)
        {
            lock (lockObject)
            {
                string otp = new Random().Next(100000, 999999).ToString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO OTP (Email, OTPCode, ExpiryTime) VALUES (@email, @otp, @exp)", conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@otp", otp);
                        cmd.Parameters.AddWithValue("@exp", DateTime.Now.AddMinutes(5));
                        cmd.ExecuteNonQuery();
                    }
                }
                return otp;
            }
        }

        public bool VerifyOTP(string email, string otp)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM OTP WHERE Email = @email AND OTPCode = @otp AND ExpiryTime > @now AND IsUsed = 0", conn))
                        {
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@otp", otp);
                            cmd.Parameters.AddWithValue("@now", DateTime.Now);

                            if ((int)cmd.ExecuteScalar() > 0)
                            {
                                using (SqlCommand updateCmd = new SqlCommand(
                                    "UPDATE OTP SET IsUsed = 1 WHERE Email = @email AND OTPCode = @otp", conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@email", email);
                                    updateCmd.Parameters.AddWithValue("@otp", otp);
                                    updateCmd.ExecuteNonQuery();
                                }
                                return true;
                            }
                        }
                    }
                }
                catch { }
                return false;
            }
        }

        public bool ResetPassword(string email, string newPassword)
        {
            lock (lockObject)
            {
                try
                {
                    
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        string hashedPassword = HashPassword(newPassword);
                        
                        
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Users SET PasswordHash = @pass WHERE Email = @email", conn))
                        {
                            cmd.Parameters.AddWithValue("@pass", hashedPassword);
                            cmd.Parameters.AddWithValue("@email", email);
                            
                            int rowsAffected = cmd.ExecuteNonQuery();
                            
                            
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public void CleanupExpiredOTPs()
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "DELETE FROM OTP WHERE ExpiryTime < @now OR IsUsed = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@now", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            }
        }



        public bool CanUserCreateRoom(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM RoomCreationLog WHERE UserId = @uid AND CreatedAt > @since", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@since", DateTime.Now.AddHours(-1));
                        return (int)cmd.ExecuteScalar() < 20;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public void LogRoomCreation(int userId, int roomCode)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO RoomCreationLog (UserId, RoomCode) VALUES (@uid, @code)", conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@code", roomCode);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            }
        }

        public int CleanupInactiveRooms(int inactiveMinutes = 15)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Rooms SET IsActive = 0 WHERE IsActive = 1 AND (LastActivity < @cutoff OR (LastActivity IS NULL AND CreatedAt < @cutoff))", conn))
                        {
                            cmd.Parameters.AddWithValue("@cutoff", DateTime.Now.AddMinutes(-inactiveMinutes));
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public bool UpdateRoomActivity(int roomCode)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Rooms SET LastActivity = GETDATE() WHERE RoomCode = @code AND IsActive = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@code", roomCode);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public int CreateRoomRecord(int roomCode, int createdBy, int? projectId = null)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        string ownerName = "";
                        using (SqlCommand getNameCmd = new SqlCommand("SELECT Username FROM Users WHERE Id = @id", conn))
                        {
                            getNameCmd.Parameters.AddWithValue("@id", createdBy);
                            ownerName = getNameCmd.ExecuteScalar()?.ToString() ?? "Unknown";
                        }
                        
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Rooms (RoomCode, OwnerId, OwnerName) VALUES (@code, @ownerId, @ownerName); SELECT SCOPE_IDENTITY();", conn))
                        {
                            cmd.Parameters.AddWithValue("@code", roomCode);
                            cmd.Parameters.AddWithValue("@ownerId", createdBy);
                            cmd.Parameters.AddWithValue("@ownerName", ownerName);
                            return Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        public bool CloseAllRooms()
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Rooms SET IsActive = 0 WHERE IsActive = 1", conn))
                        {
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<ActiveRoomInfo> GetActiveRooms(int limit = 20)
        {
            List<ActiveRoomInfo> rooms = new List<ActiveRoomInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP (@limit) r.RoomCode, r.OwnerName, r.CreatedAt, r.LastActivity,
                                 (SELECT COUNT(*) FROM RoomMembers rm WHERE rm.RoomCode = r.RoomCode AND rm.IsCurrentlyInRoom = 1) as UserCount
                          FROM Rooms r
                          WHERE r.IsActive = 1
                          ORDER BY r.LastActivity DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rooms.Add(new ActiveRoomInfo
                                {
                                    RoomCode = reader.GetInt32(0),
                                    OwnerUsername = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(2),
                                    LastActivity = reader.IsDBNull(3) ? reader.GetDateTime(2) : reader.GetDateTime(3),
                                    UserCount = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return rooms;
        }

        public bool SaveRoomChatMessage(int roomCode, int userId, string username, string message)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO RoomChatMessages (RoomCode, UserId, Username, Message) VALUES (@room, @uid, @user, @msg)", conn))
                        {
                            cmd.Parameters.AddWithValue("@room", roomCode);
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@user", username);
                            cmd.Parameters.AddWithValue("@msg", message);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<RoomChatMessage> GetRoomChatMessages(int roomCode, int limit = 100)
        {
            List<RoomChatMessage> messages = new List<RoomChatMessage>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP (@limit) UserId, Username, Message, SentAt
                          FROM RoomChatMessages
                          WHERE RoomCode = @room
                          ORDER BY SentAt ASC", conn))
                    {
                        cmd.Parameters.AddWithValue("@room", roomCode);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(new RoomChatMessage
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Message = reader.GetString(2),
                                    SentAt = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return messages;
        }

        public List<UserRoomInfo> GetUserRooms(int userId, bool activeOnly = true)
        {
            List<UserRoomInfo> rooms = new List<UserRoomInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    string query = activeOnly
                        ? @"SELECT DISTINCT rm.RoomCode, r.OwnerId, r.OwnerName, 
                                 rm.JoinedAt, r.LastActivity, r.IsActive,
                                 (SELECT COUNT(*) FROM RoomMembers rm2 WHERE rm2.RoomCode = rm.RoomCode AND rm2.IsCurrentlyInRoom = 1) as MemberCount
                            FROM RoomMembers rm
                            JOIN Rooms r ON rm.RoomCode = r.RoomCode
                            WHERE rm.UserId = @uid AND rm.IsCurrentlyInRoom = 1 AND r.IsActive = 1
                            ORDER BY r.LastActivity DESC"
                        : @"SELECT DISTINCT rm.RoomCode, r.OwnerId, r.OwnerName, 
                                 rm.JoinedAt, r.LastActivity, r.IsActive,
                                 (SELECT COUNT(*) FROM RoomMembers rm2 WHERE rm2.RoomCode = rm.RoomCode AND rm2.IsCurrentlyInRoom = 1) as MemberCount
                            FROM RoomMembers rm
                            JOIN Rooms r ON rm.RoomCode = r.RoomCode
                            WHERE rm.UserId = @uid
                            ORDER BY r.LastActivity DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rooms.Add(new UserRoomInfo
                                {
                                    RoomCode = reader.GetInt32(0),
                                    OwnerId = reader.GetInt32(1),
                                    OwnerName = reader.GetString(2),
                                    JoinedAt = reader.GetDateTime(3),
                                    LastActivity = reader.IsDBNull(4) ? reader.GetDateTime(3) : reader.GetDateTime(4),
                                    IsActive = reader.GetBoolean(5),
                                    MemberCount = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return rooms;
        }

        public List<RoomMemberInfo> GetRoomMembers(int roomCode)
        {
            List<RoomMemberInfo> members = new List<RoomMemberInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT rm.UserId, rm.Username, u.AvatarBase64, rm.JoinedAt, u.IsOnline
                          FROM RoomMembers rm
                          JOIN Users u ON rm.UserId = u.Id
                          WHERE rm.RoomCode = @room AND rm.IsCurrentlyInRoom = 1
                          ORDER BY rm.JoinedAt", conn))
                    {
                        cmd.Parameters.AddWithValue("@room", roomCode);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                members.Add(new RoomMemberInfo
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    AvatarBase64 = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    JoinedAt = reader.GetDateTime(3),
                                    IsOnline = reader.GetBoolean(4)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return members;
        }

        public bool JoinRoom(int userId, int roomCode, string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    using (SqlCommand cmd = new SqlCommand(@"
                        MERGE RoomMembers AS target
                        USING (SELECT @room AS RoomCode, @uid AS UserId) AS source
                        ON (target.RoomCode = source.RoomCode AND target.UserId = source.UserId)
                        WHEN MATCHED AND target.IsCurrentlyInRoom = 0 THEN
                            UPDATE SET IsCurrentlyInRoom = 1, JoinedAt = GETDATE(), LeftAt = NULL
                        WHEN NOT MATCHED THEN
                            INSERT (RoomCode, UserId, Username, IsCurrentlyInRoom)
                            VALUES (@room, @uid, @user, 1);
                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@room", roomCode);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.CommandTimeout = 3;
                        
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return true;
                }
                if (sqlEx.Number == -2)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool LeaveRoom(int userId, int roomCode)
        {
            lock (lockObject)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE RoomMembers SET LeftAt = GETDATE(), IsCurrentlyInRoom = 0 WHERE UserId = @uid AND RoomCode = @room AND IsCurrentlyInRoom = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@room", roomCode);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }



        public static string BitmapToBase64(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return new Bitmap(ms);
            }
        }

        public static string GenerateThumbnailBase64(Bitmap bitmap, int maxWidth = 200, int maxHeight = 150)
        {
            if (bitmap == null) return null;

            float ratio = Math.Min((float)maxWidth / bitmap.Width, (float)maxHeight / bitmap.Height);
            int newWidth = (int)(bitmap.Width * ratio);
            int newHeight = (int)(bitmap.Height * ratio);

            using (Bitmap thumbnail = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                }
                return BitmapToBase64(thumbnail);
            }
        }



        public void Dispose()
        {
            CleanupExpiredOTPs();
        }


        public bool SetProjectOnline(int projectId, int roomId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE Projects SET IsOnline = 1, RoomID = @RoomID, UpdatedAt = GETDATE() WHERE Id = @ProjectId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectId", projectId);
                        cmd.Parameters.AddWithValue("@RoomID", roomId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetProjectActiveUsers(int projectId)
        {
            List<string> users = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT u.Username FROM ProjectSessions ps INNER JOIN Users u ON ps.UserId = u.Id WHERE ps.ProjectId = @ProjectId AND ps.IsActive = 1 AND DATEDIFF(MINUTE, ps.LastActivity, GETDATE()) < 15 ORDER BY ps.JoinedAt";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectId", projectId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        { while (reader.Read()) users.Add(reader.GetString(0)); }
                    }
                }
            }
            catch { 
            }
            return users;
        }

        public bool JoinProjectSession(int projectId, int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkSql = "SELECT Id FROM ProjectSessions WHERE ProjectId = @ProjectId AND UserId = @UserId";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@ProjectId", projectId);
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        if (checkCmd.ExecuteScalar() != null)
                        {
                            string updateSql = @"UPDATE ProjectSessions SET IsActive = 1, LastActivity = GETDATE() WHERE ProjectId = @ProjectId AND UserId = @UserId";
                            using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                            { updateCmd.Parameters.AddWithValue("@ProjectId", projectId); updateCmd.Parameters.AddWithValue("@UserId", userId); updateCmd.ExecuteNonQuery(); }
                        }
                        else
                        {
                            string insertSql = @"INSERT INTO ProjectSessions (ProjectId, UserId, JoinedAt, LastActivity, IsActive) VALUES (@ProjectId, @UserId, GETDATE(), GETDATE(), 1)";
                            using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                            { insertCmd.Parameters.AddWithValue("@ProjectId", projectId); insertCmd.Parameters.AddWithValue("@UserId", userId); insertCmd.ExecuteNonQuery(); }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LeaveProjectSession(int projectId, int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE ProjectSessions SET IsActive = 0, LastActivity = GETDATE() WHERE ProjectId = @ProjectId AND UserId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    { cmd.Parameters.AddWithValue("@ProjectId", projectId); cmd.Parameters.AddWithValue("@UserId", userId); cmd.ExecuteNonQuery(); }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateProjectSessionActivity(int projectId, int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE ProjectSessions SET LastActivity = GETDATE() WHERE ProjectId = @ProjectId AND UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    { cmd.Parameters.AddWithValue("@ProjectId", projectId); cmd.Parameters.AddWithValue("@UserId", userId); cmd.ExecuteNonQuery(); }
                }
            }
            catch
            {
                return;
            }
        }


    }
}
