using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Server
{
    public static class RedisRepository
    {
        private static ConnectionMultiplexer _redis;
        private static IDatabase _db;
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            try
            {
                string connString = ConfigLoader.GetRedisConnectionString();
                if (string.IsNullOrEmpty(connString)) return;

                _redis = ConnectionMultiplexer.Connect(connString);
                _db = _redis.GetDatabase();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
            }
        }

        public static bool IsConnected => _isInitialized && _redis != null && _redis.IsConnected;

        public static void SaveOTP(string email, string otpCode)
        {
            if (!IsConnected) return;

            string key = $"otp:{email}";
            _db.StringSet(key, otpCode, TimeSpan.FromMinutes(5));

        }

        public static string GetOTP(string email)
        {
            if (!IsConnected) return null;

            string key = $"otp:{email}";
            return _db.StringGet(key);
        }

        public static void DeleteOTP(string email)
        {
            if (!IsConnected) return;

            string key = $"otp:{email}";
            _db.KeyDelete(key);
        }
    }
}
