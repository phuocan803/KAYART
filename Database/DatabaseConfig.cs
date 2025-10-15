using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Database
{

    public static class DatabaseConfig
    {
        private static JObject _config;
        private static readonly object _lock = new object();

        static DatabaseConfig()
        {
            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            try
            {
                string[] possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"),
                    Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName ?? "", "appsettings.json"),
                    Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.FullName ?? "", "appsettings.json"),
                    Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.FullName ?? "", "appsettings.json")
                };

                foreach (var configPath in possiblePaths)
                {
                    if (File.Exists(configPath))
                    {
                        string json = File.ReadAllText(configPath);
                        _config = JObject.Parse(json);
                        return; 
                    }
                }

            }
            catch (Exception ex)
            {
                // log check
            }
        }

        public static string GetConnectionString()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        string connString = _config["ConnectionStrings"]?["SqlServer"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(connString))
                        {
                            if (!connString.Contains("Connection Timeout") && !connString.Contains("Connect Timeout"))
                            {
                                connString += ";Connection Timeout=10"; // 10 second timeout
                            }
                            
                            return connString;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // log check
                }

                string errorMsg = "Database connection string not found!\n\n";
                errorMsg += "Checked locations:\n";
                errorMsg += $"1. {AppDomain.CurrentDomain.BaseDirectory}\n";
                errorMsg += "\nPlease ensure appsettings.json exists with:\n";
                errorMsg += "{\n";
                errorMsg += "  \"ConnectionStrings\": {\n";
                errorMsg += "    \"SqlServer\": \"Server=...;Database=...;User Id=...;Password=...;Connection Timeout=10;\"\n";
                errorMsg += "  }\n";
                errorMsg += "}\n\n";
                errorMsg += "Current directory: " + Directory.GetCurrentDirectory();
                
                throw new InvalidOperationException(errorMsg);
            }
        }

        public static string GetProvider()
        {
            lock (_lock)
            {
                try
                {
                    if (_config != null)
                    {
                        return _config["Database"]?["Provider"]?.Value<string>() ?? "SqlServer";
                    }
                }
                catch { }
                return "SqlServer";
            }
        }

        public static void Reload()
        {
            lock (_lock)
            {
                LoadConfiguration();
            }
        }
    }
}
