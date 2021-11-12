using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TestZXing.CustomExceptions;
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using TestZXing.Static;

namespace TestZXing.Static
{
    public static class Functions
    {
        public static DateTime NextShift(DateTime now)
        {
            //DateTime now = DateTime.Now;
            
            var x = now.ToString("H:mm");
            string[] hs = Regex.Split(x, ":");
            string h = hs[0];
            int H;
            bool parsive = int.TryParse(h, out H);
            if (parsive)
            {
                if(H<14 && H >= 6)
                {
                    //1st shift
                    return new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
                }
                else if(H<22 && H >=14)
                {
                    //2nd shift
                    return new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
                }
                else
                {
                    return DateTime.Today.AddHours(30);
                }
            }
            else
            {
                return now.AddHours(1);
            }
        }

        public static async Task CreateError(Exception ex, string text, string methodName, string className, string additionalInfo = null)
        {

            string UserName = string.Empty;

            string InternetConnectionStatus = "";
            string ActiveConnections = "";

            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                ActiveConnections += "GSM ";
            }
            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
            {
                ActiveConnections += "WiFi ";
            }


            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                InternetConnectionStatus = "Pełen internet";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet)
            {
                InternetConnectionStatus = "Ograniczone";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.Local)
            {
                InternetConnectionStatus = "Lokalne";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                InternetConnectionStatus = "Brak";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                InternetConnectionStatus = "Nieznane";
            }
            else
            {
                InternetConnectionStatus = "Tego przypadku powinno nie być... Prawdopodobnie przybyło opcji w  Connectivity.NetworkAcces.. Sprawdź Error.cs";
            }

            if (RuntimeSettings.CurrentUser != null)
            {
                UserName = RuntimeSettings.CurrentUser.FullName;
            }

            string macAddress = await DependencyService.Get<IWifiHandler>().GetWifiMacAddress();

            var ApiPing = await DependencyService.Get<IWifiHandler>().PingHost();
            string pingStatus = $"{Static.Secrets.ServerIp} : ";
            if (ApiPing) { pingStatus += "Dostępny"; } else { pingStatus += "Niedostępny"; }

            WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi();

            string _additionalInfo = "Brak";
            if(additionalInfo != null)
            {
                _additionalInfo = additionalInfo;
            }

            var properties = new Dictionary<string, string>
                {
                    {"Type", text},
                    {"Method", methodName},
                    {"Class", className},
                    {"User", UserName},
                    {"Połączenie internetowe", InternetConnectionStatus },
                    {"Aktywne połączenia", ActiveConnections },
                    {"SSID (BSSID) aktywnej sieci", wi.SSID + $"({wi.BSSID})" },
                    {"Adres MAC", macAddress },
                    {"Status pingu", pingStatus},
                    {"Dodatkowe info", _additionalInfo}
                };

            

            List<WiFiInfo> wis = await DependencyService.Get<IWifiHandler>().GetAvailableWifis(true);
            if(wis != null)
            {
                string[] status = new string[] { "" };

                foreach (WiFiInfo w in wis.OrderByDescending(i => i.Signal))
                {
                    string con = "";
                    string bssid = "";
                    if (w.IsConnected)
                    {
                        con = "(P)";
                        bssid = $" [{w.BSSID}]";
                    }
                    if(status[status.Length-1].Length + bssid.Length + w.SSID.Length + con.Length +5 > 125)
                    {
                        //create new array's item
                        Array.Resize(ref status, status.Length +1);
                    }
                    status[status.Length - 1] += w.SSID + $"{bssid} ({w.Signal}){con}, ";

                }
                for (int i = 0; i < status.Length; i++)
                {
                    properties.Add($"Status Wifi {i}", status[i]);
                }                
            }
            
            Crashes.TrackError(ex, properties);
        }

        public static async Task<HttpResponseMessage> GetPostRetryAsync(Func<Task<HttpResponseMessage>> action, TimeSpan sleepPeriod, int tryCount = 3)
        {
            int attempted = 0;
            bool pingable = false;
            CancellationTokenSource PingCts;
            CancellationTokenSource actionCts;
            HttpResponseMessage res = new HttpResponseMessage();
            Exception exc;

            if (tryCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(tryCount));

            WiFiInfo w = null;

            try
            {
                w = await DependencyService.Get<IWifiHandler>().ConnectPreferredWifi();

            }catch (WifiTurnedOffException ex)
            {
                //wifi is off
                await Application.Current.MainPage.DisplayAlert("Wyłączone WiFi", "Sieć Wifi jest wyłączona. Uruchom Wifi i spróbuj jeszcze raz.", "OK");
                throw new WifiTurnedOffException("Sieć Wifi jest wyłączona");
            }
            catch (Exception ex)
            {

            }
            



            while (true)
                {
                    try
                    {

                        attempted++;
                        if (attempted > 1)
                        {
                            DependencyService.Get<IToaster>().LongAlert($"Próba {attempted}");
                        }

                        if (RuntimeSettings.IsVpnConnection)
                        {
                            tryCount = 1;
                        }
                        else
                        {

                            var formattedSsid = $"\"{Static.Secrets.PreferredWifi}\"";
                            if (w != null)
                            {
                                if (w.SSID == formattedSsid)
                                {
                                    //tryCount = 1;
                                }
                            }
                        }


                        PingCts = new CancellationTokenSource();
                        var ping = Task.Run(() => DependencyService.Get<IWifiHandler>().PingHost(), PingCts.Token);
                        actionCts = new CancellationTokenSource();
                        var resTask = Task.Run(() => action(), actionCts.Token);

                        Task firstFinieshed = await Task.WhenAny(ping, resTask);

                        if (ping.Status == TaskStatus.RanToCompletion)
                        {
                            pingable = await ping;
                            if (!pingable)
                            {
                                actionCts.Cancel();
                                exc = new ServerUnreachableException();
                                throw exc;
                            }
                            else
                            {
                                res = await resTask;
                            }
                        }
                        else
                        {
                            PingCts.Cancel();
                            res = await resTask;
                        }


                        return res; // success!
                    }
                    catch (Exception ex)
                    {
                        --tryCount;
                        // --tryCount;
                        if (tryCount <= 0)
                        {
                            throw;
                        }
                        await Task.Delay(sleepPeriod);
                    }
                }

        }

        public static bool QuickZip(string directoryToZip, string destinationZipFullPath)
        {
            try
            {
                // Delete existing zip file if exists
                if (System.IO.File.Exists(destinationZipFullPath))
                    System.IO.File.Delete(destinationZipFullPath);

                if (!System.IO.Directory.Exists(directoryToZip))
                    return false;
                else
                {
                    System.IO.Compression.ZipFile.CreateFromDirectory(directoryToZip, destinationZipFullPath, System.IO.Compression.CompressionLevel.Optimal, true);
                    return System.IO.File.Exists(destinationZipFullPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return false;
            }
        }

        public static void CreateZipFile()
        {
            if (NLog.LogManager.IsLoggingEnabled())
            {
                string folder;

                if (Device.RuntimePlatform == Device.iOS)
                    folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "Library");
                else if (Device.RuntimePlatform == Device.Android)
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                else
                    throw new Exception("Could not show log: Platform undefined.");

                //Delete old zipfiles (housekeeping)
                try
                {
                    foreach (string fileName in System.IO.Directory.GetFiles(folder, "*.zip"))
                    {
                        System.IO.File.Delete(fileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting old zip files: {ex.Message}");
                }

                string logFolder = System.IO.Path.Combine(folder, "logs");
                if (System.IO.Directory.Exists(logFolder))
                {
                    RuntimeSettings.ZippedLogFile = $"{folder}/{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.zip";
                    int filesCount = System.IO.Directory.GetFiles(logFolder, "*.csv").Length;
                    if (filesCount > 0)
                    {
                        if (!QuickZip(logFolder, RuntimeSettings.ZippedLogFile))
                            RuntimeSettings.ZippedLogFile = string.Empty;
                    }
                    else
                        RuntimeSettings.ZippedLogFile = string.Empty;
                }
                else
                    RuntimeSettings.ZippedLogFile = string.Empty;
            }
            else
                RuntimeSettings.ZippedLogFile = string.Empty;
        }

        public static async Task<string> SaveToGallery(FileResult result, bool isPhoto)
        {
            //takes care of storing photo/video taken with camera in system gallery
            string mPath = string.Empty;
            if (isPhoto)
            {
                mPath = DependencyService.Get<IFileHandler>().GetImageGalleryPath();
            }
            else
            {
                mPath = DependencyService.Get<IFileHandler>().GetVideoGalleryPath();
            }

            string nPath = System.IO.Path.Combine(mPath, result.FileName);
            using (var stream = await result.OpenReadAsync())
            {
                using (var nStream = System.IO.File.OpenWrite(nPath))
                {
                    await stream.CopyToAsync(nStream);
                    return nPath;
                }
            }
        }

        public static async Task<string> EmbedMedia(FileResult result, bool isPhoto)
        {
            //checks if result is existent file or just captrued and decides if to store it in system gallery
            if (result != null)
            {
                string fullPath = string.Empty;

                //check if it needs to be saved first
                if (result.FullPath.Contains("com.companyname.TestZXing"))
                {
                    //save
                    fullPath = await Functions.SaveToGallery(result, isPhoto);
                }
                else
                {
                    fullPath = result.FullPath;
                }

                var stream = await result.OpenReadAsync();
                return fullPath; //ImageSource.FromStream(() => stream);

            }
            else
            {
                return null;
            }
        }

        public static async Task<string> ChangeImage()
        {
            //takes care of capturing/picking a photo/video
            //and store it in local gallery

            FileResult result = null;
            string imageUrl = string.Empty;


            try
            {
                string res = await Application.Current.MainPage.DisplayActionSheet("Co chcesz zrobić?", "Anuluj", null, "Zrób zdjęcie", "Wybierz z galerii");
                if (res == "Zrób zdjęcie")
                {
                    result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                    {
                        Title = "Zrób zdjęcie"
                    });
                }
                else if (res == "Wybierz z galerii")
                {
                    result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Wybierz zdjęcie"
                    });
                }

                if (result != null)
                {
                    string success = await Functions.EmbedMedia(result, true);
                    if (!string.IsNullOrEmpty(success))
                    {
                        imageUrl = success;
                    }

                }
            }
            catch (Exception ex)
            {
                CreateError(ex, "Error during picking/capturing/embeding/saving to gallery of image", nameof(ChangeImage), "Functions");
            }

            return imageUrl;

        }

        public static async Task OpenLatestLog()
        {
            var logName = await GetLogName();
            if(logName != null)
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(logName)
                });
            }
        }

        public static async Task<string> GetLogName()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string logFolder = System.IO.Path.Combine(folder, "logs");
            if (System.IO.Directory.Exists(logFolder))
            {
                var logs = System.IO.Directory.GetFiles(logFolder);
                string[] logNames = new string[logs.Length];
                for (int i = 0; i < logs.Length; i++)
                {
                    var logNameArray = logs[i].Split('/');
                    if (logNameArray.Length > 0)
                    {
                        logNames[i] = logNameArray[logNameArray.Length - 1];
                    }
                    else
                    {
                        logNames[i] = logs[i];
                    }
                }


                string res = await Application.Current.MainPage.DisplayActionSheet("Wybierz log to otwarcia", "Anuluj", null, logNames);
                if (res != "Anuluj")
                {
                    for (int i = 0; i < logNames.Length; i++)
                    {
                        if(res == logNames[i])
                        {
                            return logs[i];
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                DependencyService.Get<IToaster>().LongAlert($"Folder {logFolder} NIE istnieje..");
                return null;
            }
        }

        public static async Task SendLogByEmail(string path)
        {
            try
            {
                List<string> recipients = new List<string>();
                recipients.Add("robert.roszak@gmail.com");

                var message = new EmailMessage
                {
                    Subject = $"JDE Scan: Log użytkownika {RuntimeSettings.CurrentUser.FullName}",
                    Body = path,
                    To = recipients,
                };

                message.Attachments.Add(new EmailAttachment(path));
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }

    }
}
