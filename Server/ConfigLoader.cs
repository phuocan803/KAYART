using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{

    public static class ConfigLoader
    {
        private static JObject _config;
        private static readonly object _lock = new object();

        static ConfigLoader()
        {
            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Configuration file not found at: {configPath}");
                }

                string json = File.ReadAllText(configPath);
                _config = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static string GetConnectionString()
        {
            lock (_lock)
            {
                return _config["ConnectionStrings"]["SqlServer"].Value<string>();
            }
        }

        public static string GetDatabaseProvider()
        {
            lock (_lock)
            {
                return _config["Database"]["Provider"]?.Value<string>() ?? "SqlServer";
            }
        }



        public static int GetServerPort()
        {
            lock (_lock)
            {
                return _config["Server"]["Port"]?.Value<int>() ?? 9999;
            }
        }

        public static int GetMaxConnections()
        {
            lock (_lock)
            {
                return _config["Server"]["MaxConnections"]?.Value<int>() ?? 1000;
            }
        }



        public static string GetRecaptchaSecretKey()
        {
            lock (_lock)
            {
                return _config["Security"]["GoogleRecaptcha"]["SecretKey"].Value<string>();
            }
        }

        public static string GetSenderEmail()
        {
            lock (_lock)
            {
                return _config["Security"]["Email"]["SenderEmail"].Value<string>();
            }
        }

        public static string GetSenderPassword()
        {
            lock (_lock)
            {
                return _config["Security"]["Email"]["SenderPassword"].Value<string>();
            }
        }

        public static string GetSmtpHost()
        {
            lock (_lock)
            {
                return _config["Security"]["Email"]["SmtpHost"]?.Value<string>() ?? "smtp.gmail.com";
            }
        }

        public static int GetSmtpPort()
        {
            lock (_lock)
            {
                return _config["Security"]["Email"]["SmtpPort"]?.Value<int>() ?? 587;
            }
        }



        public static string GetFirebaseApiKey()
        {
            lock (_lock)
            {
                return _config["Firebase"]["ApiKey"].Value<string>();
            }
        }

        public static JObject GetFirebaseServiceAccount()
        {
            lock (_lock)
            {
                return _config["Firebase"]["ServiceAccount"] as JObject;
            }
        }

        public static string GetFirebaseServiceAccountJson()
        {
            lock (_lock)
            {
                return _config["Firebase"]["ServiceAccount"].ToString();
            }
        }



        public static bool IsFirebaseAuthEnabled()
        {
            lock (_lock)
            {
                return _config["Features"]["EnableFirebaseAuth"]?.Value<bool>() ?? true;
            }
        }

        public static bool IsEmailOTPEnabled()
        {
            lock (_lock)
            {
                return _config["Features"]["EnableEmailOTP"]?.Value<bool>() ?? true;
            }
        }

        public static int GetMaxRoomsPerHour()
        {
            lock (_lock)
            {
                return _config["Features"]["RoomRateLimit"]["MaxRoomsPerHour"]?.Value<int>() ?? 20;
            }
        }

        public static int GetRoomCleanupInterval()
        {
            lock (_lock)
            {
                return _config["Features"]["RoomRateLimit"]["CleanupIntervalMinutes"]?.Value<int>() ?? 15;
            }
        }



        public static string GetJwtSecretKey()
        {
            lock (_lock)
            {
                return _config["Jwt"]["SecretKey"].Value<string>();
            }
        }

        public static string GetJwtIssuer()
        {
            lock (_lock)
            {
                return _config["Jwt"]["Issuer"]?.Value<string>() ?? "KayArtServer";
            }
        }

        public static string GetJwtAudience()
        {
            lock (_lock)
            {
                return _config["Jwt"]["Audience"]?.Value<string>() ?? "KayArtClient";
            }
        }

        public static int GetJwtExpirationMinutes()
        {
            lock (_lock)
            {
                return _config["Jwt"]["AccessTokenExpirationMinutes"]?.Value<int>() ?? 60;
            }
        }



        public static T GetSetting<T>(string keyPath)
        {
            lock (_lock)
            {
                try
                {
                    var keys = keyPath.Split(':');
                    JToken token = _config;

                    foreach (var key in keys)
                    {
                        token = token[key];
                        if (token == null)
                            return default(T);
                    }

                    return token.Value<T>();
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public static void Reload()
        {
            lock (_lock)
            {
                LoadConfiguration();
            }
        }

        public static string GetRedisConnectionString()
        {
            lock (_lock)
            {
                return _config["Redis"]?["ConnectionString"]?.Value<string>();
            }
        }
    }
}


