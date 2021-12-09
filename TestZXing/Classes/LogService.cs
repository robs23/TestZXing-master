using Microsoft.AppCenter.Crashes;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;
using File = TestZXing.Models.File;

namespace TestZXing.Classes
{
    public class LogService : ILogService
    {
        public UserLogKeeper UserLogKeeper { get; set; }

        public LogService()
        {
            UserLogKeeper = new UserLogKeeper();
        }

        public void Initialize(Assembly assembly, string assemblyName)
        {
            string resourcePrefix;
            if (Device.RuntimePlatform == Device.iOS)
                resourcePrefix = "TestZXing.iOS";
            else if (Device.RuntimePlatform == Device.Android)
                resourcePrefix = "TestZXing.Droid";
            else
                throw new Exception("Could not initialize Logger: Unknonw Platform");
            //var location = $"{assemblyName}.NLog.config";
            string location = $"{resourcePrefix}.NLog.config";
            Stream stream = assembly.GetManifestResourceStream(location);
            if (stream == null)
                throw new Exception($"The resource '{location}' was not loaded properly.");
            LogManager.Configuration = new XmlLoggingConfiguration(System.Xml.XmlReader.Create(stream), null);
        }

        public string CreateSnapshotOfCurrentLog()
        {
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string path = "";
            string logsFolder = RuntimeSettings.LogsFolderPath;
            string filesFolder = RuntimeSettings.FilesFolderPath;

            if (!Directory.Exists(filesFolder))
            {
                //create it first
                Directory.CreateDirectory(filesFolder);
            }
            string fileName = Path.Combine(logsFolder, "nLog.csv");
            if (System.IO.File.Exists(fileName))
            {
                //copy file
                path = Path.Combine(filesFolder, newFileName);
                System.IO.File.Copy(fileName, path);
            }
            else
            {
                path = $"Plik {fileName} nie został znaleziony..";
            }

            return path;
        }

        public async Task ReportLastSessionCrash()
        {
            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                //Functions.CreateZipFile();
                var report = await Crashes.GetLastSessionCrashReportAsync();
                if (report != null)
                {
                    string macAddress = await DependencyService.Get<IWifiHandler>().GetWifiMacAddress();

                    UserLog u = new UserLog()
                    {
                        HasTheAppCrashed = true,
                        OnRequest = false,
                        LogName = report.AppErrorTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Platform = $"{DeviceInfo.Platform.ToString()} {DeviceInfo.VersionString}",
                        Device = $"{DeviceInfo.Manufacturer} {DeviceInfo.Model} {macAddress}",
                        Comment = null,
                        ErrorTime = report.AppErrorTime.DateTime,
                        Message = MessageFromStackTrace(report.StackTrace),
                        StackTrace = report.StackTrace,
                        CreatedOn = DateTime.Now,
                        CreatedBy = RuntimeSettings.CurrentUser.UserId,
                        TenantId = RuntimeSettings.CurrentUser.TenantId
                    };
                    UserLogKeeper.Items.Add(u);

                    await UserLogKeeper.AddToSyncQueue();

                    File f = new File()
                    {

                    };
                }
            }
        }



        private string MessageFromStackTrace(string stacktTrace)
        {
            string message = stacktTrace;
            if (!string.IsNullOrEmpty(message))
            {
                string[] ats = Regex.Split(message, " at ");
                if (ats.Length > 1)
                {
                    message = ats[0];
                }
            }
            return message;
        }
    }
}
