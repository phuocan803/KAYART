using System;

namespace Client
{
    /// lưu token trong memory không persist vào disk
    internal static class AuthManager
    {
        // lưu JWT token trong memory
        public static string CurrentToken { get; private set; }
        public static int CurrentUserId { get; private set; }
        public static string CurrentUsername { get; private set; }
        public static string CurrentRole { get; private set; }
        public static DateTime? TokenExpiry { get; private set; }

        /// lưu token  khi đăng nhập thành công
        public static void SaveToken(string token, int userId, string username, string role = "User", DateTime? expiry = null)
        {
            CurrentToken = token;
            CurrentUserId = userId;
            CurrentUsername = username;
            CurrentRole = role;
            TokenExpiry = expiry;
        }

        /// xóa token khi đăng xuất
        public static void ClearToken()
        {
            CurrentToken = null;
            CurrentUserId = 0;
            CurrentUsername = null;
            CurrentRole = null;
            TokenExpiry = null;
        }

        /// Kiểm tra token có hợp lệ hay không
        public static bool IsTokenValid()
        {
            if (string.IsNullOrEmpty(CurrentToken))
                return false;

            if (TokenExpiry.HasValue && TokenExpiry.Value <= DateTime.UtcNow)
                return false;

            return true;
        }

        /// Kiểm tra token sắp hết hạn 
        public static bool IsTokenExpiringSoon()
        {
            if (!TokenExpiry.HasValue)
                return false;

            var timeLeft = TokenExpiry.Value - DateTime.UtcNow;
            return timeLeft.TotalMinutes < 5;
        }

        /// get time còn lại của token
        public static TimeSpan? GetTimeUntilExpiry()
        {
            if (!TokenExpiry.HasValue)
                return null;

            return TokenExpiry.Value - DateTime.UtcNow;
        }

        /// get token dạng string để hiển thị
        public static string GetTokenInfo()
        {
            if (!IsTokenValid())
                return "No valid token";

            var timeLeft = GetTimeUntilExpiry();
            if (timeLeft.HasValue)
            {
                return $"User: {CurrentUsername} (ID:{CurrentUserId}), Expires in: {timeLeft.Value.TotalMinutes:F0} minutes";
            }

            return $"User: {CurrentUsername} (ID:{CurrentUserId})";
        }
    }
}
