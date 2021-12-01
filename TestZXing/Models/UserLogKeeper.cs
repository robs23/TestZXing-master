using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TestZXing.Static;
using SQLite;
using TestZXing.Interfaces;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text.RegularExpressions;

namespace TestZXing.Models
{
    public class UserLogKeeper : Keeper<UserLog>, IOfflineKeeper, IOfflineKeeper<UserLog>
    {
        protected override string ObjectName => "UserLog";

        protected override string PluralizedObjectName => "UserLogs";

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
                    Items.Add(u);

                    await AddToSyncQueue();
                }
            }
        }

        private string MessageFromStackTrace(string stacktTrace)
        {
            string message = stacktTrace;
            if (!string.IsNullOrEmpty(message))
            {
                string[] ats = Regex.Split(message, " at ");
                if(ats.Length > 1)
                {
                    message = ats[0];
                }
            }
            return message;
        }

    }
}
