using System;

namespace Database
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarBase64 { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarBase64 { get; set; }
        public bool IsOnline { get; set; }
    }

    public class FriendInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AvatarBase64 { get; set; }
        public bool IsOnline { get; set; }
    }



    public class Project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ImageData { get; set; }
        public string ThumbnailData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SharedProject
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string OwnerUsername { get; set; }
        public string Permission { get; set; }
        public string ThumbnailData { get; set; }
        public DateTime SharedAt { get; set; }
    }

    public class SharedUserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AvatarBase64 { get; set; }
        public string Permission { get; set; }
        public DateTime SharedAt { get; set; }
    }



    public class ChatMessage
    {
        public int FromUserId { get; set; }
        public string FromUsername { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }



    public class ActiveRoomInfo
    {
        public int RoomCode { get; set; }
        public string OwnerUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public int UserCount { get; set; }
    }

    public class RoomChatMessage
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class UserRoomInfo
    {
        public int RoomCode { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
    }

    public class RoomMemberInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AvatarBase64 { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsOnline { get; set; }
    }

}
