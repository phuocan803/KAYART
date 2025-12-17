using Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using StackExchange.Redis;

namespace Server
{
    public partial class Server : Form
    {
        private List<Room> roomList = new List<Room>();
        private List<User> userList = new List<User>();
        private TcpListener listener;
        private Thread clientListener;
        private Manager Manager;
        private DatabaseHelper dbHelper;
        private System.Timers.Timer cleanupTimer;

        private readonly object roomListLock = new object();
        private readonly object userListLock = new object();
        private volatile bool isStopping = false;

        public Server()
        {
            InitializeComponent();
            SafeSetIcon();

            Manager = new Manager(listView_log, textBox_room_count, textBox_user_count);
            dbHelper = new DatabaseHelper();

            Manager.WriteToLog("Server initialized - Using SQL Server database");

            cleanupTimer = new System.Timers.Timer(60000);
            cleanupTimer.Elapsed += CleanupTimer_Elapsed;
            cleanupTimer.AutoReset = true;
        }

        private void button_start_server_Click(object sender, EventArgs e)
        {
            try
            {
                isStopping = false;
                listener = new TcpListener(IPAddress.Any, 9999);
                listener.Start();

                clientListener = new Thread(Listen);
                clientListener.IsBackground = true;
                clientListener.Start();

                RedisRepository.Initialize();
                Manager.WriteToLog("Start listening for incoming requests...");

                cleanupTimer?.Start();

                this.button_start_server.Enabled = false;
                this.button_stop_server.Enabled = true;
            }
            catch (Exception ex)
            {
                Manager.ShowError($"Failed to start server: {ex.Message}");
                Manager.WriteToLog($"[ERROR] Start server failed: {ex.Message}");
            }
        }

        private void button_stop_server_Click(object sender, EventArgs e)
        {
            try
            {
                isStopping = true;

                Manager.WriteToLog("Stopping server...");

                cleanupTimer?.Stop();

                listener?.Stop();

                if (clientListener != null && clientListener.IsAlive)
                {
                    if (!clientListener.Join(2000))
                    {
                        try
                        {
                            clientListener.Abort();
                        }
                        catch { }
                    }
                }

                lock (userListLock)
                {
                    foreach (User user in userList.ToList())
                    {
                        try
                        {
                            user.Client?.Close();
                        }
                        catch { }
                    }
                    userList.Clear();
                }

      

                Manager.WriteToLog("Server stopped");
                Manager.UpdateRoomCount(0);
                Manager.UpdateUserCount(0);

                this.button_stop_server.Enabled = false;
                this.button_start_server.Enabled = true;
            }
            catch (Exception ex)
            {
                Manager.ShowError($"Error stopping server: {ex.Message}");
                Manager.WriteToLog($"[ERROR] Stop server: {ex.Message}");
            }
        }

        private void button_get_server_IP_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = Manager.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
                if (string.IsNullOrEmpty(ip))
                    ip = Manager.GetLocalIPv4(NetworkInterfaceType.Ethernet);

                textBox_server_local_IP.Text = ip ?? "Unknown";

                if (!string.IsNullOrEmpty(ip))
                {
                    Manager.WriteToLog($"Server IP: {ip}");
        }
            }
            catch (Exception ex)
            {
                Manager.ShowError($"Error getting IP: {ex.Message}");
                textBox_server_local_IP.Text = "Error";
            }
        }

        private void Listen()
        {
            try
            {
                while (!isStopping)
                {
                    try
                    {
                        var client = listener.AcceptTcpClient();

                    Thread receiver = new Thread(Receive);
                    receiver.IsBackground = true;
                    receiver.Start(client);
                }
                    catch (SocketException)
                    {
                        if (!isStopping)
                        {
                            Manager.WriteToLog("[WARNING] Socket exception in Listen, continuing...");
            }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!isStopping)
                {
                    Manager.WriteToLog($"[ERROR] Listen thread crashed: {ex.Message}");
            }
        }
        }

        private void Receive(object obj)
        {
            TcpClient client = obj as TcpClient;
            if (client == null) return;

            User user = new User(client);

            lock (userListLock)
            {
                userList.Add(user);
            }

            try
            {
                while (!isStopping)
                {
                    string requestInJson = user.Reader.ReadLine();

                    if (requestInJson == null)
                    {
                        break;
                    }

                    Packet request = null;
                    try
                    {
                        request = JsonConvert.DeserializeObject<Packet>(requestInJson);
                    }
                    catch (JsonException ex)
                    {
                        Manager.WriteToLog($"[ERROR] Invalid JSON from {user.Username ?? "unknown"}: {ex.Message}");
                        continue;
                    }

                    if (request == null) continue;

                    try
                    {
                    switch (request.Code)
                    {
                        case 0:
                            generate_room_handler(user, request);
                            break;
                        case 1:
                            join_room_handler(user, request);
                            break;
                        case 2:
                            sync_bitmap_handler(user, request);
                            break;
                        case 3:
                            send_bitmap_handler(user, request);
                            break;
                        case 4:
                            send_graphics_handler(user, request);
                            break;
                        case 5:
                            send_message_handler(user, request);
                            break;
                        case 6:
                            Login_handler(user, request);
                            break;
                        case 7:
                            SignUp_handler(user, request);
                            break;
                        case 8:
                            ForgotPassword_handler(user, request);
                            break;
                        case 9:
                            VerifyOTP_handler(user, request);
                            break;
                        case 10:
                            ResetPassword_handler(user, request);
                            break;
                        case 11:
                            GetUserProjects_handler(user, request);
                            break;
                        case 12:
                            GetFriends_handler(user, request);
                            break;
                        case 13:
                            GetSharedProjects_handler(user, request);
                            break;
                        case 14:
                            GetActiveRooms_handler(user, request);
                            break;
                        case 15:
                            GetUserAvatar_handler(user, request);
                            break;
                        case 16:
                            UpdateUserAvatar_handler(user, request);
                            break;
                        case 17:
                            ShareProject_handler(user, request);
                            break;
                        case 18:
                            GetProjectShares_handler(user, request);
                            break;
                        case 19:
                            DeleteProject_handler(user, request);
                            break;
                        case 20:
                            GetProjectById_handler(user, request);
                            break;
                        case 21:
                            UnshareProject_handler(user, request);
                            break;
                        case 22:
                            GetFriendList_handler(user, request);
                            break;
                        case 23:
                            GetChatMessages_handler(user, request);
                            break;
                        case 24:
                            SendChatMessage_handler(user, request);
                            break;
                        case 25:
                            SearchUsers_handler(user, request);
                            break;
                        case 26:
                            SendFriendRequest_handler(user, request);
                            break;
                        case 27:
                            GetRoomChatMessages_handler(user, request);
                            break;
                        case 28:
                            SaveRoomChatMessage_handler(user, request);
                            break;
                        case 29:
                            GetRoomMembers_handler(user, request);
                            break;
                        case 30:
                            JoinRoomTracking_handler(user, request);
                            break;
                        case 31:
                            LeaveRoomTracking_handler(user, request);
                            break;
                        case 32:
                            GetUserIdByUsername_handler(user, request);
                            break;
                        case 33:
                            CreateProject_handler(user, request);
                            break;
                        case 34:
                            UpdateProjectImage_handler(user, request);
                            break;
                        case 35:
                            UpdateUserOnlineStatus_handler(user, request);
                            break;
                        case 40:
                            MakeProjectOnline_handler(user, request);
                            break;
                        case 41:
                            GetProjectActiveUsers_handler(user, request);
                            break;
                        case 42:
                            JoinProjectRoom_handler(user, request);
                            break;
                        case 43:
                            LeaveProjectRoom_handler(user, request);
                            break;

                        case 51:
                            Logout_handler(user, request);
                            break;
                        default:
                            Manager.WriteToLog($"[WARNING] Unknown packet code: {request.Code}");
                            break;
                    }
                }
                    catch (Exception ex)
                    {
                        Manager.WriteToLog($"[ERROR] Handler exception for code {request.Code}: {ex.Message}");
            }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                if (!isStopping && !string.IsNullOrEmpty(user.Username))
                {
                    Manager.WriteToLog($"[INFO] {user.Username} connection closed unexpectedly");
                }
            }
            catch (SocketException sockEx)
            {
                if (!isStopping && !string.IsNullOrEmpty(user.Username))
                {
                    Manager.WriteToLog($"[INFO] {user.Username} socket error: {sockEx.Message}");
                }
            }
            catch (Exception ex)
            {
                if (!isStopping)
                {
                    Manager.WriteToLog($"[ERROR] Receive error for {user.Username ?? "unknown"}: {ex.GetType().Name} - {ex.Message}");
                }
            }
            finally
            {
                close_client(user);
            }
        }

        private void generate_room_handler(User user, Packet request)
        {
            user.Username = request.Username;

            int userId = dbHelper.GetUserIdByUsername(request.Username);
            if (userId > 0)
            {
                if (!dbHelper.CanUserCreateRoom(userId))
                {
                    Manager.WriteToLog($"[RATE LIMIT] {user.Username} exceeded 20 rooms/hour limit");
                    Packet errorMessage = new Packet
                    {
                        Code = 0,
                        Success = false,
                        ErrorMessage = "Rate limit exceeded: Maximum 20 rooms per hour. Please wait."
                    };
                    sendSpecific(user, errorMessage);
                    return;
                }
            }

            Random r = new Random();
            int roomID = r.Next(1000, 9999);

            lock (roomListLock)
            {
                while (roomList.Any(room => room.roomID == roomID))
                {
                    roomID = r.Next(1000, 9999);
                }
            }

            Room newRoom = new Room();
            newRoom.roomID = roomID;
            newRoom.userList.Add(user);

            lock (roomListLock)
            {
                roomList.Add(newRoom);
            }

            if (userId > 0)
            {
                try
                {
                    dbHelper.CreateRoomRecord(roomID, userId, null);
                    dbHelper.LogRoomCreation(userId, roomID);
                    Manager.WriteToLog($"{user.Username} (ID:{userId}) created room {roomID} - Logged to DB");
                }
                catch (Exception ex)
                {
                    Manager.WriteToLog($"[WARNING] Failed to log room creation: {ex.Message}");
                }
            }
            else
            {
                Manager.WriteToLog(user.Username + " created new room. Room code: " + newRoom.roomID);
            }

            UpdateCounts();

            Packet message = new Packet
            {
                Code = 0,
                Username = request.Username,
                RoomID = roomID.ToString(),
                Success = true
            };

            sendSpecific(user, message);
        }

        private void join_room_handler(User user, Packet request)
        {
            bool roomExist = false;
            Room requestingRoom = null;

            if (!int.TryParse(request.RoomID?.ToString(), out int id))
            {
                Manager.WriteToLog($"[ERROR] Invalid room ID: {request.RoomID}");
                request.Username = "err:thisroomdoesnotexist";
                sendSpecific(user, request);
                return;
            }

            lock (roomListLock)
            {
                requestingRoom = roomList.FirstOrDefault(room => room.roomID == id);
                roomExist = requestingRoom != null;
            }

            if (!roomExist)
            {
                request.Username = "err:thisroomdoesnotexist";
                sendSpecific(user, request);
                return;
            }

            try
            {
                dbHelper.UpdateRoomActivity(id);
            }
            catch { }

            user.Username = request.Username;

            try
            {
                int userId = dbHelper.GetUserIdByUsername(request.Username);
                if (userId > 0)
                {
                    dbHelper.JoinRoom(userId, id, request.Username);
                    Manager.WriteToLog($"[DATABASE] {request.Username} (ID:{userId}) joined room {id}");
                }
            }
            catch (Exception ex)
            {
                Manager.WriteToLog($"[WARNING] Failed to track room join in DB: {ex.Message}");
            }

            lock (requestingRoom.userList)
            {
                requestingRoom.userList.Add(user);
                request.Username = requestingRoom.GetUsernameListInString();

                foreach (User _user in requestingRoom.userList.ToList())
                {
                    sendSpecific(_user, request);
                }
            }

            Manager.WriteToLog("Room " + request.RoomID + ": " + user.Username + " joined");
            UpdateCounts();
        }

        private void sync_bitmap_handler(User user, Packet request)
        {
            if (!int.TryParse(request.RoomID?.ToString(), out int id))
            {
                Manager.WriteToLog($"[ERROR] Invalid room ID in sync_bitmap: {request.RoomID}");
                return;
            }

            Room requestingRoom = null;

            lock (roomListLock)
                {
                requestingRoom = roomList.FirstOrDefault(room => room.roomID == id);
                }

            if (requestingRoom == null)
            {
                Manager.WriteToLog($"[ERROR] Room {id} not found for sync_bitmap");
                return;
            }

            User firstUser = null;
            lock (requestingRoom.userList)
            {
                if (requestingRoom.userList.Count > 0)
                {
                    firstUser = requestingRoom.userList[0];
        }
            }

            if (firstUser != null)
            {
                sendSpecific(firstUser, request);
            }
        }

        private void send_bitmap_handler(User user, Packet request)
        {
            if (!int.TryParse(request.RoomID?.ToString(), out int id))
            {
                Manager.WriteToLog($"[ERROR] Invalid room ID in send_bitmap: {request.RoomID}");
                return;
            }

            Room requestingRoom = null;

            lock (roomListLock)
                {
                requestingRoom = roomList.FirstOrDefault(room => room.roomID == id);
                }

            if (requestingRoom == null)
            {
                Manager.WriteToLog($"[ERROR] Room {id} not found for send_bitmap");
                return;
            }

            User lastUser = null;
            lock (requestingRoom.userList)
            {
                if (requestingRoom.userList.Count > 0)
                {
                    lastUser = requestingRoom.userList[requestingRoom.userList.Count - 1];
        }
            }

            if (lastUser != null)
            {
                sendSpecific(lastUser, request);
            }
        }

        private void send_graphics_handler(User user, Packet request)
        {
            if (!int.TryParse(request.RoomID?.ToString(), out int id))
            {
                Manager.WriteToLog($"[ERROR] Invalid room ID in send_graphics: {request.RoomID}");
                return;
            }

            Room requestingRoom = null;

            lock (roomListLock)
                {
                requestingRoom = roomList.FirstOrDefault(room => room.roomID == id);
                }

            if (requestingRoom == null)
            {
                Manager.WriteToLog($"[ERROR] Room {id} not found for send_graphics");
                return;
            }

            List<User> usersToSend;
            lock (requestingRoom.userList)
            {
                usersToSend = requestingRoom.userList.Where(u => u != user).ToList();
            }

            foreach (User _user in usersToSend)
                {
                    sendSpecific(_user, request);
                }
            }

        private void send_message_handler(User user, Packet request)
        {
            if (!int.TryParse(request.RoomID?.ToString(), out int id))
            {
                Manager.WriteToLog($"[ERROR] Invalid room ID in send_message: {request.RoomID}");
                return;
            }

            Room requestingRoom = null;

            lock (roomListLock)
            {
                requestingRoom = roomList.FirstOrDefault(room => room.roomID == id);
            }

            if (requestingRoom == null)
            {
                Manager.WriteToLog($"[ERROR] Room {id} not found for send_message");
                return;
            }

            try
            {
                int userId = dbHelper.GetUserIdByUsername(request.Username);
                if (userId > 0)
                {
                    dbHelper.SaveRoomChatMessage(id, userId, request.Username, request.BitmapString);
                }
            }
            catch (Exception ex)
            {
                Manager.WriteToLog($"[WARNING] Failed to save room message to DB: {ex.Message}");
            }

            List<User> usersToSend;
            lock (requestingRoom.userList)
            {
                usersToSend = requestingRoom.userList.ToList();
            }

            foreach (User _user in usersToSend)
            {
                sendSpecific(_user, request);
            }

            Manager.WriteToLog($"Room {request.RoomID}: {user.Username} sent message: {request.BitmapString}");
        }


        private bool ValidateJwtToken(User user, Packet request)
        {
            if (string.IsNullOrEmpty(request.JwtToken))
            {
                Packet response = new Packet { Code = request.Code, Success = false, ErrorMessage = "Missing authentication token" };
                sendSpecific(user, response);
                return false;
            }

            try
            {
                JwtHelper jwtHelper = new JwtHelper(
                    ConfigLoader.GetJwtSecretKey(),
                    ConfigLoader.GetJwtIssuer(),
                    ConfigLoader.GetJwtAudience(),
                    ConfigLoader.GetJwtExpirationMinutes()
                );

                jwtHelper.ValidateToken(request.JwtToken);
                
                return true;
            }
            catch (Exception)
            {
                Packet response = new Packet { Code = request.Code, Success = false, ErrorMessage = "Login session expired or invalid" };
                sendSpecific(user, response);
                return false;
            }
        }

        private async void Login_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 6 };

            bool isHuman = await Manager.VerifyCaptcha(request.CaptchaToken);

            if (!isHuman)
            {
                response.Success = false;
                response.ErrorMessage = "Captcha verification failed! Please try again.";
                Manager.WriteToLog($"[LOGIN BLOCKED] Suspicious login attempt from {user.Client.Client.RemoteEndPoint}");
                sendSpecific(user, response);
                return; 
            }
            try
            {
                bool success = dbHelper.LoginUser(request.LoginUsername, request.LoginPassword);

                if (success)
                {
                    int userId = dbHelper.GetUserIdByUsername(request.LoginUsername);
                    string role = "User";

                    try
                    {
                        JwtHelper jwtHelper = new JwtHelper(
                            secretKey: ConfigLoader.GetJwtSecretKey(),
                            issuer: ConfigLoader.GetJwtIssuer(),
                            audience: ConfigLoader.GetJwtAudience(),
                            expirationMinutes: ConfigLoader.GetJwtExpirationMinutes()
                        );

                        string jwtToken = jwtHelper.GenerateToken(userId, request.LoginUsername, role);
                        DateTime expiry = DateTime.UtcNow.AddMinutes(ConfigLoader.GetJwtExpirationMinutes());

                        response.Success = true;
                        response.LoginUsername = request.LoginUsername;
                        response.JwtToken = jwtToken;
                        response.UserId = userId;
                        response.Role = role;
                        response.TokenExpiry = expiry;
                        response.ErrorMessage = null;

                        Manager.WriteToLog($"[LOGIN SUCCESS] {request.LoginUsername} (ID:{userId}) - JWT token generated");
                    }
                    catch (Exception jwtEx)
                    {
                        Manager.WriteToLog($"[JWT ERROR] Failed to generate token: {jwtEx.Message}");
                        response.Success = false;
                        response.ErrorMessage = "Error creating authentication token!";
                    }
                }
                else
                {
                    response.Success = false;
                    response.LoginUsername = request.LoginUsername;
                    response.ErrorMessage = "Username or password is incorrect!";
                    Manager.WriteToLog($"[LOGIN FAILED] {request.LoginUsername}");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Server error: " + ex.Message;
                Manager.WriteToLog($"[LOGIN ERROR] {ex.Message}");
            }

            sendSpecific(user, response);
        }

        private void Logout_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 51 };
            try
            {
                Manager.WriteToLog($"[LOGOUT] User {request.LoginUsername ?? request.Username} logged out");
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[LOGOUT ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private async void SignUp_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 7 };
            bool isHuman = await Manager.VerifyCaptcha(request.CaptchaToken);
            if (!isHuman)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid Captcha!";
                sendSpecific(user, response);
                return;
            }
            try
            {
                bool success = dbHelper.RegisterUser(
                    request.SignUpUsername,
                    request.SignUpPassword,
                    request.SignUpEmail,
                    request.SignUpFullName,
                    request.SignUpPhoneNumber
                );

                response.Success = success;
                response.SignUpUsername = request.SignUpUsername;
                response.ErrorMessage = success ? null : "Username or Email already exists!";

                Manager.WriteToLog($"[SIGNUP] {request.SignUpUsername} - {(success ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Server error: " + ex.Message;
                Manager.WriteToLog($"[SIGNUP ERROR] {ex.Message}");
            }

            sendSpecific(user, response);
        }

        private async void ForgotPassword_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 8 };
            bool isHuman = await Manager.VerifyCaptcha(request.CaptchaToken);
            if (!isHuman)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid Captcha!";
                sendSpecific(user, response);
                return;
            }
            try
            {
                if (dbHelper.CheckEmailExists(request.ForgotPasswordEmail))
                {
                    Random r = new Random();
                    string otpCode = r.Next(100000, 999999).ToString();
                    RedisRepository.SaveOTP(request.ForgotPasswordEmail, otpCode);
                    string magicLinkUrl = await Manager.GenerateMagicLink(request.ForgotPasswordEmail);

                    await Manager.SendHybridEmail(request.ForgotPasswordEmail, otpCode, magicLinkUrl);

                    response.Success = true;
                    response.ForgotPasswordEmail = request.ForgotPasswordEmail;
                    response.ErrorMessage = "Verification code sent! Please check your email.";

                    Manager.WriteToLog($"[OTP SENT] {request.ForgotPasswordEmail} | OTP: {otpCode}");
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "Email does not exist in the system!";
                    Manager.WriteToLog($"[FORGOT PASSWORD] Email not found: {request.ForgotPasswordEmail}");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Server error: " + ex.Message;
                Manager.WriteToLog($"[FORGOT PASSWORD ERROR] {ex.Message}");
            }

            sendSpecific(user, response);
        }

        private void VerifyOTP_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 9 };

            try
            {
                string savedOtp = RedisRepository.GetOTP(request.VerifyOTPEmail);

                bool isMatch = false;
                bool success = dbHelper.VerifyOTP(request.VerifyOTPEmail, request.VerifyOTPCode);
                if (!string.IsNullOrEmpty(savedOtp) && savedOtp == request.VerifyOTPCode)
                {
                    isMatch = true;
                }

                if (isMatch)
                {
                    RedisRepository.DeleteOTP(request.VerifyOTPEmail);

                    response.Success = true;
                    response.VerifyOTPEmail = request.VerifyOTPEmail;
                    Manager.WriteToLog($"[OTP VERIFIED] {request.VerifyOTPEmail}");
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = string.IsNullOrEmpty(savedOtp)
                        ? "OTP code has expired (5 minutes). Please get a new code."
                        : "OTP code is incorrect!";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Server error: " + ex.Message;
                Manager.WriteToLog($"[VERIFY OTP ERROR] {ex.Message}");
            }

            sendSpecific(user, response);
        }

        private void ResetPassword_handler(User user, Packet request)
        {
            Packet response = new Packet { Code = 10 };

            try
            {
                bool success = dbHelper.ResetPassword(request.ResetPasswordEmail, request.ResetPasswordNewPassword);
                response.Success = success;
                response.ResetPasswordEmail = request.ResetPasswordEmail;
                response.ErrorMessage = success ? null : "Cannot reset password!";

                Manager.WriteToLog($"[RESET PASSWORD] {request.ResetPasswordEmail} - {(success ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "Server error: " + ex.Message;
                Manager.WriteToLog($"[RESET PASSWORD ERROR] {ex.Message}");
            }

            sendSpecific(user, response);
        }


        private void close_client(User user)
        {
            Room requestingRoom = null;

            lock (roomListLock)
            {
                foreach (Room room in roomList.ToList())
            {
                if (room.userList.Contains(user))
                {
                    requestingRoom = room;
                        lock (room.userList)
                        {
                    room.userList.Remove(user);
                        }
                    break;
                }
            }
            }

            if (requestingRoom != null && !string.IsNullOrEmpty(user.Username))
            {
                try
                {
                    int userId = dbHelper.GetUserIdByUsername(user.Username);
                    if (userId > 0)
                    {
                        dbHelper.LeaveRoom(userId, requestingRoom.roomID);
                        Manager.WriteToLog($"[DATABASE] {user.Username} (ID:{userId}) left room {requestingRoom.roomID}");
                    }
                }
                catch (Exception ex)
                {
                    Manager.WriteToLog($"[WARNING] Failed to track room leave in DB: {ex.Message}");
                }
            }

            lock (userListLock)
            {
            userList.Remove(user);
            }

            try
            {
                user.Client?.Close();
            }
            catch { }

            if (!string.IsNullOrEmpty(user.Username))
            {
                Manager.WriteToLog(user.Username + " disconnected.");
            }

            if (requestingRoom != null)
            {
                bool roomIsEmpty = false;
                lock (requestingRoom.userList)
                {
                    roomIsEmpty = requestingRoom.userList.Count == 0;
                }

                if (roomIsEmpty)
            {
                    lock (roomListLock)
            {
                if (roomList.Contains(requestingRoom))
                {
                            Manager.WriteToLog($"Room {requestingRoom.roomID} is now empty - will close after 15min of inactivity");
                            
                            try
                            {
                                dbHelper?.UpdateRoomActivity(requestingRoom.roomID);
                            }
                            catch { }
                }
            }
                }
            else
            {
                    Packet message = new Packet()
                    {
                        Code = 1,
                        Username = "!" + user.Username
                    };

                    List<User> usersToNotify;
                    lock (requestingRoom.userList)
                    {
                        usersToNotify = requestingRoom.userList.ToList();
                    }

                    foreach (User _user in usersToNotify)
                {
                    sendSpecific(_user, message);
                }
                    
                    try
                    {
                        dbHelper?.UpdateRoomActivity(requestingRoom.roomID);
                    }
                    catch { }
            }
            }

            UpdateCounts();
        }

        private void sendSpecific(User user, Object message)
        {
            if (user == null || user.Writer == null) return;

            string messageInJson = JsonConvert.SerializeObject(message);
            try
            {
                lock (user.Writer)
                {
                user.Writer.WriteLine(messageInJson);
                user.Writer.Flush();
            }
            }
            catch (Exception ex)
            {
                Manager.WriteToLog($"[ERROR] Cannot send data to user {user.Username ?? "unknown"}: {ex.Message}");
            }
        }

        private void UpdateCounts()
        {
            int roomCount, userCount;

            lock (roomListLock)
            {
                roomCount = roomList.Count;
            }

            lock (userListLock)
            {
                userCount = userList.Count;
            }

            Manager.UpdateRoomCount(roomCount);
            Manager.UpdateUserCount(userCount);
        }

        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                
                isStopping = true;

                cleanupTimer?.Stop();
                cleanupTimer?.Dispose();

                if (dbHelper != null)
                {
                    dbHelper.CleanupExpiredOTPs();
                    try
                    {
                        dbHelper.CloseAllRooms();
                    }
                    catch { }
                }

                if (listener != null)
                {
                    try
                    {
                        listener.Stop();
                    }
                    catch { }
                }

                if (clientListener != null && clientListener.IsAlive)
                {
                    if (!clientListener.Join(2000))
                    {
                        try
                        {
                            clientListener.Abort();
                        }
                        catch { }
                    }
                }

                lock (userListLock)
                {
                    foreach (User user in userList.ToList())
                    {
                        try
                        {
                            user.Client?.Close();
                        }
                        catch { }
                    }
                    userList.Clear();
                }

                lock (roomListLock)
                {
                    roomList.Clear();
                }

                Manager.WriteToLog("Server closed successfully");
                
                if (Application.OpenForms.Count <= 1)
                {
                    System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                    {
                        Application.Exit();
                        Environment.Exit(0);
                    });
                }
            }
            catch (Exception ex)
            {
                Manager?.WriteToLog($"[ERROR] Error during shutdown: {ex.Message}");
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private void SafeSetIcon()
        {
            try
            {
                var logo = Properties.Resources.Logo;
                if (logo is Bitmap bmp && bmp != null)
                {
                    IntPtr hIcon = IntPtr.Zero;
                    Icon tmpIcon = null;
                    try
                    {
                        hIcon = bmp.GetHicon();
                        tmpIcon = Icon.FromHandle(hIcon);
                        this.Icon = (Icon)tmpIcon.Clone();
                    }
                    finally
                    {
                        tmpIcon?.Dispose();
                        if (hIcon != IntPtr.Zero)
                        {
                            DestroyIcon(hIcon);
                        }
                    }
                }
        }
            catch
        {
            }
        }

        private void CleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                int cleanedRooms = dbHelper.CleanupInactiveRooms(15);
                if (cleanedRooms > 0)
                {
                    Manager.WriteToLog($"[CLEANUP] Marked {cleanedRooms} inactive rooms as closed");
                }
            }
            catch (Exception ex)
            {
                Manager.WriteToLog($"[ERROR] Cleanup timer failed: {ex.Message}");
            }
        }



        private void GetUserProjects_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 11 };
            try
            {
                var projects = dbHelper.GetUserProjects(request.RequestUserId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(projects);
                Manager.WriteToLog($"[CODE 11] Get User Projects: User {request.RequestUserId} - {projects.Count} projects");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 11 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetFriends_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 12 };
            try
            {
                var friends = dbHelper.GetFriendList(request.RequestUserId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(friends);
                Manager.WriteToLog($"[CODE 12] Get Friends: User {request.RequestUserId} - {friends.Count} friends");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 12 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetSharedProjects_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 13 };
            try
            {
                var sharedProjects = dbHelper.GetSharedProjects(request.RequestUserId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(sharedProjects);
                Manager.WriteToLog($"[CODE 13] Get Shared Projects: User {request.RequestUserId} - {sharedProjects.Count} projects");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 13 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetActiveRooms_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 14 };
            try
            {
                var activeRooms = dbHelper.GetActiveRooms(request.MaxRooms);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(activeRooms);
                Manager.WriteToLog($"[CODE 14] Get Active Rooms: {activeRooms.Count} rooms");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 14 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetUserAvatar_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 15 };
            try
            {
                string avatar = dbHelper.GetUserAvatar(request.RequestUserId);
                response.Success = true;
                response.AvatarBase64 = avatar;
                Manager.WriteToLog($"[CODE 15] Get User Avatar: User {request.RequestUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 15 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void UpdateUserAvatar_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 16 };
            try
            {
                bool success = dbHelper.UpdateUserAvatar(request.RequestUserId, request.AvatarBase64);
                response.Success = success;
                Manager.WriteToLog($"[CODE 16] Update User Avatar: User {request.RequestUserId} - {(success ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 16 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void ShareProject_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 17 };
            try
            {
                bool success = dbHelper.ShareProject(request.ProjectId, request.TargetUserId, request.Permission);
                response.Success = success;
                Manager.WriteToLog($"[CODE 17] Share Project: Project {request.ProjectId} → User {request.TargetUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 17 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetProjectShares_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 18 };
            try
            {
                var shares = dbHelper.GetProjectShares(request.ProjectId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(shares);
                Manager.WriteToLog($"[CODE 18] Get Project Shares: Project {request.ProjectId} - {shares.Count} shares");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 18 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void DeleteProject_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 19 };
            try
            {
                bool success = dbHelper.DeleteProject(request.ProjectId);
                response.Success = success;
                Manager.WriteToLog($"[CODE 19] Delete Project: Project {request.ProjectId} - {(success ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 19 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetProjectById_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 20 };
            try
            {
                var project = dbHelper.GetProjectById(request.ProjectId);
                response.Success = project != null;
                response.ResponseData = JsonConvert.SerializeObject(project);
                Manager.WriteToLog($"[CODE 20] Get Project By Id: Project {request.ProjectId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 20 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void UnshareProject_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 21 };
            try
            {
                bool success = dbHelper.UnshareProject(request.ProjectId, request.TargetUserId);
                response.Success = success;
                Manager.WriteToLog($"[CODE 21] Unshare Project: Project {request.ProjectId} ← User {request.TargetUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 21 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }



        private void GetFriendList_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 22 };
            try
            {
                var friends = dbHelper.GetFriendList(request.RequestUserId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(friends);
                Manager.WriteToLog($"[CODE 22] Get Friend List: User {request.RequestUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 22 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetChatMessages_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 23 };
            try
            {
                var messages = dbHelper.GetChatMessages(request.RequestUserId, request.FriendId, request.MessageLimit);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(messages);
                Manager.WriteToLog($"[CODE 23] Get Chat Messages: User {request.RequestUserId} ↔ Friend {request.FriendId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 23 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void SendChatMessage_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 24 };
            try
            {
                bool success = dbHelper.SendChatMessage(request.RequestUserId, request.ToUserId, request.ChatMessage);
                response.Success = success;
                Manager.WriteToLog($"[CODE 24] Send Chat Message: User {request.RequestUserId} → User {request.ToUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 24 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void SearchUsers_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 25 };
            try
            {
                var users = dbHelper.SearchUsers(request.SearchTerm, request.RequestUserId, request.SearchLimit);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(users);
                Manager.WriteToLog($"[CODE 25] Search Users: '{request.SearchTerm}' - {users.Count} results");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 25 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void SendFriendRequest_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 26 };
            try
            {
                bool success = dbHelper.SendFriendRequest(request.RequestUserId, request.FriendUserId);
                response.Success = success;
                Manager.WriteToLog($"[CODE 26] Send Friend Request: User {request.RequestUserId} → User {request.FriendUserId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 26 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetRoomChatMessages_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 27 };
            try
            {
                var messages = dbHelper.GetRoomChatMessages(request.RoomCode, request.MessageLimit);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(messages);
                Manager.WriteToLog($"[CODE 27] Get Room Chat Messages: Room {request.RoomCode}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 27 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void SaveRoomChatMessage_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 28 };
            try
            {
                bool success = dbHelper.SaveRoomChatMessage(request.RoomCode, request.RequestUserId, request.Username, request.ChatMessage);
                response.Success = success;
                Manager.WriteToLog($"[CODE 28] Save Room Chat Message: Room {request.RoomCode} - User {request.Username}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 28 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void GetRoomMembers_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 29 };
            try
            {
                var members = dbHelper.GetRoomMembers(request.RoomCode);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(members);
                Manager.WriteToLog($"[CODE 29] Get Room Members: Room {request.RoomCode}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 29 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void JoinRoomTracking_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 30 };
            try
            {
                bool success = dbHelper.JoinRoom(request.RequestUserId, request.RoomCode, request.Username);
                response.Success = success;
                Manager.WriteToLog($"[CODE 30] Join Room Tracking: User {request.Username} → Room {request.RoomCode}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 30 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void LeaveRoomTracking_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 31 };
            try
            {
                bool success = dbHelper.LeaveRoom(request.RequestUserId, request.RoomCode);
                response.Success = success;
                Manager.WriteToLog($"[CODE 31] Leave Room Tracking: User {request.RequestUserId} ← Room {request.RoomCode}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 31 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }



        private void GetUserIdByUsername_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 32 };
            try
            {
                int userId = dbHelper.GetUserIdByUsername(request.Username);
                response.Success = userId > 0;
                response.RequestUserId = userId;
                Manager.WriteToLog($"[CODE 32] Get User ID: {request.Username} → ID {userId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 32 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void CreateProject_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 33 };
            try
            {
                int projectId = dbHelper.CreateProject(request.RequestUserId, request.ProjectName, request.ImageData, request.ThumbnailData, request.ImageWidth, request.ImageHeight);
                response.Success = projectId > 0;
                response.ProjectId = projectId;
                Manager.WriteToLog($"[CODE 33] Create Project: '{request.ProjectName}' by User {request.RequestUserId} → ID {projectId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 33 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void UpdateProjectImage_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 34 };
            try
            {
                bool success = dbHelper.UpdateProjectImage(request.ProjectId, request.ImageData, request.ThumbnailData);
                response.Success = success;
                Manager.WriteToLog($"[CODE 34] Update Project Image: Project {request.ProjectId}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 34 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void UpdateUserOnlineStatus_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 35 };
            try
            {
                bool success = dbHelper.UpdateUserOnlineStatus(request.RequestUserId, request.IsOnline);
                response.Success = success;
                Manager.WriteToLog($"[CODE 35] Update User Online Status: User {request.RequestUserId} → {(request.IsOnline ? "ONLINE" : "OFFLINE")}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                Manager.WriteToLog($"[CODE 35 ERROR] {ex.Message}");
            }
            sendSpecific(user, response);
        }

        private void MakeProjectOnline_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 40 };
            try
            {
                int roomCode = request.ProjectId;
                int roomId = dbHelper.CreateRoomRecord(roomCode, request.RequestUserId, request.ProjectId);
                
                if (roomId > 0)
                {
                    bool success = dbHelper.SetProjectOnline(request.ProjectId, roomId);
                    if (success)
                    {
                        Room newRoom = new Room();
                        newRoom.roomID = roomCode;
                        newRoom.userList.Add(user);
                        lock (roomListLock) { roomList.Add(newRoom); }
                        dbHelper.JoinProjectSession(request.ProjectId, request.RequestUserId);
                        response.Success = true;
                        response.RoomID = roomCode.ToString();
                        Manager.WriteToLog($"[CODE 40] Project {request.ProjectId} made online → Room {roomCode}");
                    }
                    else { response.Success = false; response.ErrorMessage = "Failed to set project online"; }
                }
                else { response.Success = false; response.ErrorMessage = "Failed to create room"; }
            }
            catch (Exception ex) { response.Success = false; response.ErrorMessage = ex.Message; Manager.WriteToLog($"[CODE 40 ERROR] {ex.Message}"); }
            sendSpecific(user, response);
        }

        private void GetProjectActiveUsers_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 41 };
            try
            {
                var activeUsers = dbHelper.GetProjectActiveUsers(request.ProjectId);
                response.Success = true;
                response.ResponseData = JsonConvert.SerializeObject(activeUsers);
                Manager.WriteToLog($"[CODE 41] Get Active Users: Project {request.ProjectId} → {activeUsers.Count} users");
            }
            catch (Exception ex) { response.Success = false; response.ErrorMessage = ex.Message; Manager.WriteToLog($"[CODE 41 ERROR] {ex.Message}"); }
            sendSpecific(user, response);
        }

        private void JoinProjectRoom_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 42 };
            try
            {
                bool success = dbHelper.JoinProjectSession(request.ProjectId, request.RequestUserId);
                if (success)
                {
                    int roomCode = request.ProjectId;
                    Room requestingRoom = null;
                    lock (roomListLock) { requestingRoom = roomList.FirstOrDefault(room => room.roomID == roomCode); }
                    if (requestingRoom != null)
                    {
                        lock (requestingRoom.userList) { if (!requestingRoom.userList.Contains(user)) requestingRoom.userList.Add(user); }
                        response.Success = true;
                        response.RoomID = roomCode.ToString();
                        Manager.WriteToLog($"[CODE 42] User {request.RequestUserId} joined project room {roomCode}");
                    }
                    else { response.Success = false; response.ErrorMessage = "Project room not found"; }
                }
                else { response.Success = false; response.ErrorMessage = "Failed to join project session"; }
            }
            catch (Exception ex) { response.Success = false; response.ErrorMessage = ex.Message; Manager.WriteToLog($"[CODE 42 ERROR] {ex.Message}"); }
            sendSpecific(user, response);
        }

        private void LeaveProjectRoom_handler(User user, Packet request)
        {
            if (!ValidateJwtToken(user, request)) return;

            Packet response = new Packet { Code = 43 };
            try
            {
                bool success = dbHelper.LeaveProjectSession(request.ProjectId, request.RequestUserId);
                if (success)
                {
                    int roomCode = request.ProjectId;
                    Room requestingRoom = null;
                    lock (roomListLock) { requestingRoom = roomList.FirstOrDefault(room => room.roomID == roomCode); }
                    if (requestingRoom != null) { lock (requestingRoom.userList) { requestingRoom.userList.Remove(user); } }
                    response.Success = true;
                    Manager.WriteToLog($"[CODE 43] User {request.RequestUserId} left project room {roomCode}");
                }
                else { response.Success = false; response.ErrorMessage = "Failed to leave project session"; }
            }
            catch (Exception ex) { response.Success = false; response.ErrorMessage = ex.Message; Manager.WriteToLog($"[CODE 43 ERROR] {ex.Message}"); }
            sendSpecific(user, response);
        }

    }
}        
