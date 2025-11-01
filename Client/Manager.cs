using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Database;
using System.Threading;
using System.Collections.Generic; 

namespace Client
{
    public class Manager
    {
        ListView List;
        TextBox RoomID;
        
        private TcpClient dataClient;
        private StreamReader dataReader;
        private StreamWriter dataWriter;
        private readonly SemaphoreSlim requestSemaphore = new SemaphoreSlim(1, 1); 
        private bool isDataConnected = false;

        public Manager(ListView list, TextBox roomID)
        {
            this.List = list;
            this.RoomID = roomID;
        }


        /// thêm username vào danh sách user
        public void AddToUserListView(string line)
        {
            if (List.InvokeRequired)
            {
                List.Invoke(new Action(() =>
                {
                    List.Items.Add(line);
                }));
            }
            else
            {
                List.Items.Add(line);
            }
        }

        // xóa username khỏi danh sách user
        public void RemoveFromUserListView(string line)
        {
            Action action = () =>
            {
                foreach (ListViewItem item in List.Items)
                {
                    if (item.Text == line)
                    {
                        List.Items.Remove(item);
                        break;
                    }
                }
            };
            if (List.InvokeRequired)
            {
                List.Invoke(action);
            }
            else
            {
                action();
            }
        }

        // xóa tất cả user khỏi danh sách, giữ lại dòng đầu tiên
        public void ClearUserListView()
        {
            Action action = () =>
            {
                // giữ lại dòng đầu tiên, tiêu đề :>
                if (List.Items.Count > 0)
                {
                    ListViewItem firstLine = List.Items[0];
                    List.Clear();
                    List.Items.Add(firstLine);
                }
                else
                {
                    List.Clear();
                }
            };
            if (List.InvokeRequired)
            {
                List.Invoke(action);
            }
            else
            {
                action();
            }
        }

        // cập nhật room id hiển thị trên box ở panel
        public void UpdateRoomID(string roomID)
        {
            if (RoomID.InvokeRequired)
            {
                RoomID.Invoke(new Action(() =>
                {
                    RoomID.Text = "Room: " + roomID;
                }));
            }
            else
            {
                RoomID.Text = "Room: " + roomID;
            }
        }

        // chuyển bitmap thành string base64
        public string BitmapToString(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        // chuyển string base64 thành bitmap
        public Bitmap StringToBitmap(string base64string)
        {
            if (string.IsNullOrEmpty(base64string)) return null;
            
            byte[] imageBytes = Convert.FromBase64String(base64string);
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(imageBytes))
            {
                return new Bitmap(Image.FromStream(stream));
            }
        }


   
        public async Task<bool> ConnectDataChannelAsync(string serverIP, int port = 9999)
        {
            if (isDataConnected) return true;

            try
            {
                if (string.IsNullOrWhiteSpace(serverIP))
                {
                    return false;
                }

                if (port <= 0 || port > 65535)
                {
                    return false;
                }

                dataClient = new TcpClient();
                dataClient.NoDelay = true;
                
                var connectTask = dataClient.ConnectAsync(serverIP, port);
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(5000)); 
                
                if (completedTask != connectTask)
                {
                    dataClient?.Close();
                    return false;
                }

                NetworkStream stream = dataClient.GetStream();
                stream.ReadTimeout = 10000; 
                stream.WriteTimeout = 10000;
                
                dataReader = new StreamReader(stream);
                dataWriter = new StreamWriter(stream) { AutoFlush = true };

                isDataConnected = true;
                return true;
            }
            catch (OperationCanceledException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void DisconnectDataChannel()
        {
            try
            {
                isDataConnected = false;
                dataReader?.Dispose();
                dataWriter?.Dispose();
                dataClient?.Close();
            }
            catch { }
        }

        private async Task<Packet> SendDataRequestAsync(Packet request)
        {
            if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
            {
                request.JwtToken = AuthManager.CurrentToken;
            }

            if (!isDataConnected)
            {
                throw new Exception("Data channel not connected");
            }

            await requestSemaphore.WaitAsync();
            try
            {
                if (dataWriter == null || dataReader == null)
                {
                    throw new Exception("Data channel streams are null");
                }

                string json = JsonConvert.SerializeObject(request);
                await dataWriter.WriteLineAsync(json);
                await dataWriter.FlushAsync();

                var cts = new System.Threading.CancellationTokenSource(10000); 
                string responseJson = null;
                
                try
                {
                    responseJson = await Task.Run(() => dataReader.ReadLineAsync(), cts.Token);
                }
                catch (OperationCanceledException)
                {
                    throw new Exception("Server response timeout");
                }

                if (responseJson == null)
                {
                    throw new Exception("Server disconnected");
                }

                return JsonConvert.DeserializeObject<Packet>(responseJson);
            }
            catch (Exception ex)
            {
                isDataConnected = false;
                throw;
            }
            finally
            {
                requestSemaphore.Release();
            }
        }

       
        /// Code 11: Get User Projects
        public async Task<List<Project>> GetUserProjectsAsync(int userId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 11, RequestUserId = userId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<Project>>(response.ResponseData);
                }
                return new List<Project>();
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
        }

        /// Code 12: Get Friends
        public async Task<List<FriendInfo>> GetFriendsAsync(int userId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 12, RequestUserId = userId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<FriendInfo>>(response.ResponseData);
                }
                return new List<FriendInfo>();
            }
            catch (Exception ex)
            {
                return new List<FriendInfo>();
            }
        }

        /// Code 13: Get Shared Projects
        public async Task<List<SharedProject>> GetSharedProjectsAsync(int userId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 13, RequestUserId = userId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<SharedProject>>(response.ResponseData);
                }
                return new List<SharedProject>();
            }
            catch (Exception ex)
            {
                return new List<SharedProject>();
            }
        }

        /// Code 14: Get Active Rooms
        public async Task<List<ActiveRoomInfo>> GetActiveRoomsAsync(int maxRooms = 20)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 14, MaxRooms = maxRooms });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<ActiveRoomInfo>>(response.ResponseData);
                }
                return new List<ActiveRoomInfo>();
            }
            catch (Exception ex)
            {
                return new List<ActiveRoomInfo>();
            }
        }

        /// Code 15: Get User Avatar
        public async Task<string> GetUserAvatarAsync(int userId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 15, RequestUserId = userId });
                return response.Success ? response.AvatarBase64 : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// Code 16: Update User Avatar
        public async Task<bool> UpdateUserAvatarAsync(int userId, string avatarBase64)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 16, 
                    RequestUserId = userId, 
                    AvatarBase64 = avatarBase64 
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 17: Share Project
        public async Task<bool> ShareProjectAsync(int projectId, int targetUserId, string permission = "view")
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 17, 
                    ProjectId = projectId, 
                    TargetUserId = targetUserId, 
                    Permission = permission 
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 18: Get Project Shares
        public async Task<List<SharedUserInfo>> GetProjectSharesAsync(int projectId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 18, ProjectId = projectId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<SharedUserInfo>>(response.ResponseData);
                }
                return new List<SharedUserInfo>();
            }
            catch (Exception ex)
            {
                return new List<SharedUserInfo>();
            }
        }

        /// Code 19: Delete Project
        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 19, ProjectId = projectId });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 20: Get Project By Id
        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 20, ProjectId = projectId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<Project>(response.ResponseData);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// Code 21 - Unshare Project
        public async Task<bool> UnshareProjectAsync(int projectId, int targetUserId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 21, 
                    ProjectId = projectId, 
                    TargetUserId = targetUserId
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// Code 22: Get Friend List
        public async Task<List<FriendInfo>> GetFriendListAsync(int userId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 22, RequestUserId = userId });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<FriendInfo>>(response.ResponseData);
                }
                return new List<FriendInfo>();
            }
            catch (Exception ex)
            {
                return new List<FriendInfo>();
            }
        }

        /// Code 23: Get Chat Messages
        public async Task<List<ChatMessage>> GetChatMessagesAsync(int userId, int friendId, int limit = 50)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 23, 
                    RequestUserId = userId, 
                    FriendId = friendId,
                    MessageLimit = limit
                });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<ChatMessage>>(response.ResponseData);
                }
                return new List<ChatMessage>();
            }
            catch (Exception ex)
            {
                return new List<ChatMessage>();
            }
        }

        /// Code 24: Send Chat Message
        public async Task<bool> SendChatMessageAsync(int fromUserId, int toUserId, string message)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 24, 
                    RequestUserId = fromUserId,
                    ToUserId = toUserId,
                    ChatMessage = message
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 25: Search Users
        public async Task<List<UserInfo>> SearchUsersAsync(string searchTerm, int currentUserId, int limit = 10)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 25, 
                    RequestUserId = currentUserId,
                    SearchTerm = searchTerm,
                    SearchLimit = limit
                });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<UserInfo>>(response.ResponseData);
                }
                return new List<UserInfo>();
            }
            catch (Exception ex)
            {
                return new List<UserInfo>();
            }
        }

        /// Code 26: Send Friend Request
        public async Task<bool> SendFriendRequestAsync(int userId, int friendUserId)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 26, 
                    RequestUserId = userId,
                    FriendUserId = friendUserId
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MANAGER] SendFriendRequest error: {ex.Message}");
                return false;
            }
        }

        /// Code 27: Get Room Chat Messages
        public async Task<List<RoomChatMessage>> GetRoomChatMessagesAsync(int roomCode, int limit = 100)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 27, 
                    RoomCode = roomCode,
                    MessageLimit = limit
                });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<RoomChatMessage>>(response.ResponseData);
                }
                return new List<RoomChatMessage>();
            }
            catch (Exception ex)
            {
                return new List<RoomChatMessage>();
            }
        }

        /// Code 28: Save Room Chat Message
        public async Task<bool> SaveRoomChatMessageAsync(int roomCode, int userId, string username, string message)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 28, 
                    RoomCode = roomCode,
                    RequestUserId = userId,
                    Username = username,
                    ChatMessage = message
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 29: Get Room Members
        public async Task<List<RoomMemberInfo>> GetRoomMembersAsync(int roomCode)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet { Code = 29, RoomCode = roomCode });
                if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                {
                    return JsonConvert.DeserializeObject<List<RoomMemberInfo>>(response.ResponseData);
                }
                return new List<RoomMemberInfo>();
            }
            catch (Exception ex)
            {
                return new List<RoomMemberInfo>();
            }
        }

        /// Code 30: Join Room
        public async Task<bool> JoinRoomAsync(int userId, int roomCode, string username)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 30, 
                    RequestUserId = userId,
                    RoomCode = roomCode,
                    Username = username
                });

                if (response.Success && userId > 0)
                {
                    try
                    {
                        await UpdateUserOnlineStatusAsync(userId, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 31: Leave Room
        public async Task<bool> LeaveRoomAsync(int userId, int roomCode)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 31, 
                    RequestUserId = userId,
                    RoomCode = roomCode
                });

                if (response.Success && userId > 0)
                {
                    try
                    {
                        await UpdateUserOnlineStatusAsync(userId, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// Code 32: Get User ID by Username
        public async Task<int> GetUserIdByUsernameAsync(string username)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 32, 
                    Username = username
                });
                return response.Success ? response.RequestUserId : -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// Code 33: Create Project
        public async Task<int> CreateProjectAsync(int userId, string projectName, string imageData, string thumbnailData, int width, int height)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 33,
                    RequestUserId = userId,
                    ProjectName = projectName,
                    ImageData = imageData,
                    ThumbnailData = thumbnailData,
                    ImageWidth = width,
                    ImageHeight = height
                });
                return response.Success ? response.ProjectId : -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// Code 34: Update Project Image
        public async Task<bool> UpdateProjectImageAsync(int projectId, string imageData, string thumbnailData)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 34,
                    ProjectId = projectId,
                    ImageData = imageData,
                    ThumbnailData = thumbnailData
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// Code 35: Update User Online Status
        public async Task<bool> UpdateUserOnlineStatusAsync(int userId, bool isOnline)
        {
            try
            {
                var response = await SendDataRequestAsync(new Packet 
                { 
                    Code = 35,
                    RequestUserId = userId,
                    IsOnline = isOnline
                });
                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
