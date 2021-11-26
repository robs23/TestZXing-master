using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;

namespace TestZXing.Static
{
    public static class RuntimeSettings
    {
        public static int TenantId { get; set; }
        public static int UserId { get; set; }
        public static User CurrentUser { get; set; }
        public static string LocalDbPath { get; set; }
        public static string LocalApplicationFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string LogsFolderPath { get; set; } = System.IO.Path.Combine(LocalApplicationFolder, "logs");
        public static string OfflineFilesPath
        {
            get
            {
                string offlineFiles = System.IO.Path.Combine(LocalApplicationFolder, "offlineFiles");
                if (!System.IO.Directory.Exists(offlineFiles))
                {
                    //create folder
                    System.IO.Directory.CreateDirectory(offlineFiles);
                }
                return offlineFiles;
            }
        }
        public static string ConnectionErrorText { get { return "Nie można połączyć się z serwerem. Prawdopodobnie utraciłeś połączenie internetowe. Upewnij się, że masz połączenie z internetem i spróbuj jeszcze raz"; } }
        public static string ConnectionErrorTitle { get { return "Brak połączenia"; } }

        public static string ThumbnailsPath { get { return "Files/Thumbnails/"; } }
        public static string FilesPath { get { return "Files/"; } }

        public static bool IsVpnConnection { get; set; } = true;

        public static string ZippedLogFile { get; set; }
        public static FileKeeper UploadKeeper { get; set; }
        public static UserLogKeeper UserLogSyncKeeper { get; set; }
        public static SyncKeeper SyncKeeper { get; set; }

        public static string ImagePlaceholderName { get { return "image_placeholder_128.png"; } }

        public static bool IsWifiEnablingFinished { get; set; } = false;
        public static bool IsWifiRequestingFinished { get; set; } = false;
    }
}
