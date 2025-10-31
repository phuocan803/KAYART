using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Client
{
    internal class Packet
    {
        public int Code { get; set; }
        public string Username { get; set; }
        public string RoomID { get; set; }
        public string PenColor { get; set; }
        public float PenWidth { get; set; }
        public int ShapeTag { get; set; }
        public List<Point> Points_1 { get; set; }
        public List<Point> Points_2 { get; set; }
        public float[] Position { get; set; }
        public string BitmapString { get; set; }

        public string JwtToken { get; set; }
        public DateTime TokenExpiry { get; set; }

        // Code 6: Login
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string CaptchaToken { get; set; }
        public string Role { get; set; }

        // Code 7: SignUp
        public string SignUpUsername { get; set; }
        public string SignUpPassword { get; set; }
        public string SignUpEmail { get; set; }
        public string SignUpFullName { get; set; }
        public string SignUpPhoneNumber { get; set; }

        // Code 8: ForgotPassword
        public string ForgotPasswordEmail { get; set; }
        public string ForgotPasswordPhoneNumber { get; set; }
        public bool IsPhoneVerification { get; set; }

        // Code 9: VerifyOTP
        public string VerifyOTPEmail { get; set; }
        public string VerifyOTPCode { get; set; }

        // Code 10: ResetPassword
        public string ResetPasswordEmail { get; set; }
        public string ResetPasswordNewPassword { get; set; }

        // Code 11: Get User Projects
        public int RequestUserId { get; set; }
        public int UserId { get; set; }

        // Code 12: Get Friends
        // (uses RequestUserId)

        // Code 13: Get Shared Projects
        // (uses RequestUserId)

        // Code 14: Get Active Rooms
        public int MaxRooms { get; set; } = 20;

        // Code 15: Get User Avatar
        // (uses RequestUserId)

        // Code 16: Update User Avatar
        public string AvatarBase64 { get; set; }

        // Code 17: Share Project
        public int ProjectId { get; set; }
        public int TargetUserId { get; set; }
        public string Permission { get; set; }

        // Code 18: Get Project Shares
        // (uses ProjectId)

        // Code 19: Delete Project
        // (uses ProjectId)

        // Code 20: Get Project By Id
        // (uses ProjectId)

        // Code 21 - Unshare Project
        // (uses ProjectId, TargetUserId)

        // Code 22: Get Friend List
        // (uses RequestUserId)

        // Code 23: Get Chat Messages
        public int FriendId { get; set; }
        public int MessageLimit { get; set; } = 50;

        // Code 24: Send Chat Message
        public int ToUserId { get; set; }
        public string ChatMessage { get; set; }

        // Code 25: Search Users
        public string SearchTerm { get; set; }
        public int SearchLimit { get; set; } = 10;

        // Code 26: Send Friend Request
        public int FriendUserId { get; set; }

        // Code 27: Get Room Chat Messages
        public int RoomCode { get; set; }

        // Code 28: Save Room Chat Message
        // (uses RoomCode, RequestUserId, Username, ChatMessage)

        // Code 29: Get Room Members
        // (uses RoomCode)

        // Code 30: Join Room
        // (uses RoomCode, RequestUserId, Username)

        // Code 31: Leave Room
        // (uses RoomCode, RequestUserId)

        // Code 32: Get User ID by Username
        // (uses Username)

        // Code 33: Create Project
        public string ProjectName { get; set; }
        public string ImageData { get; set; }
        public string ThumbnailData { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        // Code 34: Update Project Image
        // (uses ProjectId, ImageData, ThumbnailData)

        // Code 35: Update User Online Status
        public bool IsOnline { get; set; }

        // Code 40: Make Project Online (uses ProjectId, RequestUserId)
        // Code 41: Get Project Active Users (uses ProjectId)
        // Code 42: Join Project Room (uses ProjectId, RequestUserId)
        // Code 43: Leave Project Room (uses ProjectId, RequestUserId)

        public string ResponseData { get; set; }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string GeneratedOTP { get; set; } // test
    }
}
