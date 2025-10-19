using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.IO;

namespace Server
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

        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string CaptchaToken { get; set; }
        public string Role { get; set; }

        public string SignUpUsername { get; set; }
        public string SignUpPassword { get; set; }
        public string SignUpEmail { get; set; }
        public string SignUpFullName { get; set; }
        public string SignUpPhoneNumber { get; set; }

        public string ForgotPasswordEmail { get; set; }
        public string ForgotPasswordPhoneNumber { get; set; } 
        public bool IsPhoneVerification { get; set; }

        public string VerifyOTPEmail { get; set; }
        public string VerifyOTPCode { get; set; }

        public string ResetPasswordEmail { get; set; }
        public string ResetPasswordNewPassword { get; set; }

        public int RequestUserId { get; set; }
        public int UserId { get; set; }
        
        
        
        public int MaxRooms { get; set; } = 20;
        
        
        public string AvatarBase64 { get; set; }
        
        public int ProjectId { get; set; }
        public int TargetUserId { get; set; }
        public string Permission { get; set; }
        
        
        

        
        public int FriendId { get; set; }
        public int MessageLimit { get; set; } = 50;
        
        public int ToUserId { get; set; }
        public string ChatMessage { get; set; }
        
        public string SearchTerm { get; set; }
        public int SearchLimit { get; set; } = 10;
        
        public int FriendUserId { get; set; }
        
        public int RoomCode { get; set; }
        
        
        
        

        
        public string ProjectName { get; set; }
        public string ImageData { get; set; }
        public string ThumbnailData { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        

        public bool IsOnline { get; set; }

        public string ResponseData { get; set; }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string GeneratedOTP { get; set; }
    }



    internal class User
    {
        public TcpClient Client { get; set; }
        public string Username { get; set; }
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }

        public User(TcpClient client)
        {
            this.Client = client;
            this.Username = string.Empty;
            NetworkStream stream = Client.GetStream();
            this.Reader = new StreamReader(stream);
            this.Writer = new StreamWriter(stream);
        }
    }



    internal class Room
    {
        public int roomID;
        public List<User> userList = new List<User>();

        public string GetUsernameListInString()
        {
            List<string> usernames = new List<string>();
            foreach (User user in userList)
            {
                usernames.Add(user.Username);
            }
            string[] s = usernames.ToArray();
            string res = string.Join(",", s);

            return res;
        }
    }

}
