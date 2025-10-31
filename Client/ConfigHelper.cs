using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Client
{
    public static class ConfigHelper
    {
        private static JObject _config;
        private static readonly object _lock = new object();

        // mặc định là cái này nếu không có config
        private const string DEFAULT_SERVER_IP = "127.0.0.1";
        private const int DEFAULT_SERVER_PORT = 9999;
        private const string DEFAULT_RECAPTCHA_KEY = ""; // để test

        static ConfigHelper()
        {
            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _config = JObject.Parse(json);
                    }
                    else
                    {
                        _config = null;
                    }
                }
                else
                {
                    _config = null;
                }
            }
            catch (Exception ex)
            {
                _config = null;
            }
        }

        //; get ip từ config
        public static string GetServerIP()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        string configValue = _config["Server"]?["DefaultIP"]?.Value<string>();
                        
                        if (!string.IsNullOrWhiteSpace(configValue))
                        {
                            return configValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    /// ngoại lệ thì nó đã get default rồi
                }

                return DEFAULT_SERVER_IP;
            }
        }

        /// get port từ config
        public static int GetServerPort()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        int? configValue = _config["Server"]?["DefaultPort"]?.Value<int?>();
                        
                        if (configValue.HasValue && configValue.Value > 0 && configValue.Value <= 65535)
                        {
                            return configValue.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // tương tự
                }
                
                return DEFAULT_SERVER_PORT;
            }
        }

        /// get reCAPTCHA site key từ config
        public static string GetReCaptchaSiteKey()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        string configValue = _config["ReCaptcha"]?["SiteKey"]?.Value<string>();
                        
                        if (!string.IsNullOrWhiteSpace(configValue) && 
                            configValue != "YOUR_RECAPTCHA_SITE_KEY_HERE" &&
                            !configValue.Contains("PASTE_YOUR"))
                        {
                            return configValue;
                        }
                        else
                        {
                            // log check 
                        }
                    }
                    else
                    {
                        // log check
                    }
                }
                catch (Exception ex)
                {
                    // log check
                }

                // trả về default
                return DEFAULT_RECAPTCHA_KEY;
            }
        }

         /// check config null
        public static bool IsConfigValid()
        {
            lock (_lock)
            {
                return _config != null;
            }
        }

        /// mode 
        public static bool IsLocalMode()
        {
            string ip = GetServerIP();
            int port = GetServerPort();
            
            return ip == DEFAULT_SERVER_IP && port == DEFAULT_SERVER_PORT;
        }

        /// log check
        public static string GetConfigSummary()
        {
            return $"Server: {GetServerIP()}:{GetServerPort()}, " +
                   $"Config Valid: {IsConfigValid()}, " +
                   $"Local Mode: {IsLocalMode()}";
        }

        /// reload config
        public static void Reload()
        {
            lock (_lock)
            {
                LoadConfiguration();
            }
        }


        /// get api key ai từ config
        public static string GetStabilityApiKey()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        string apiKey = _config["StabilityAI"]?["ApiKey"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(apiKey) && apiKey != "YOUR_STABILITY_API_KEY_HERE")
                        {
                            return apiKey;
                        }
                    }
                }
                catch { }
                return null;
            }
        }
    }
}
