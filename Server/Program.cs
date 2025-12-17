using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                FirebaseSetup.Initialize();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Firebase Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }
            Application.Run(new Server());
        }
    }
    public static class FirebaseSetup
    {
        public static void Initialize()
        {
            try
            {
                string serviceAccountJson = ConfigLoader.GetFirebaseServiceAccountJson();
                
                if (!string.IsNullOrWhiteSpace(serviceAccountJson))
                {
                    System.Diagnostics.Debug.WriteLine("[FIREBASE] Initializing from appsettings.json");
                    
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(serviceAccountJson),
                    });
                    
                    System.Diagnostics.Debug.WriteLine("[FIREBASE] Initialized successfully");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FIREBASE] Error loading from config: {ex.Message}");
                
                throw new InvalidOperationException(
                    "Firebase configuration not found in appsettings.json!\n\n" +
                    "Please add Firebase.ServiceAccount section to appsettings.json\n" +
                    "See appsettings.example.json for the required format.",
                    ex
                );
            }
            
            throw new InvalidOperationException(
                "Firebase configuration is empty or invalid in appsettings.json!\n\n" +
                "Please configure Firebase.ServiceAccount section properly."
            );
        }
    }
}
