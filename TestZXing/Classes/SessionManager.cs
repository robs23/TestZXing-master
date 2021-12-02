using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TestZXing.Interfaces;
using TestZXing.Static;
using TestZXing.Views;
using Xamarin.Forms;

namespace TestZXing.Classes
{
    public sealed class SessionManager
    {
        /// <summary>
        /// Implementation based on https://stackoverflow.com/questions/41466761/xamarin-forms-check-if-user-inactive-after-some-time-log-out-app answer
        /// </summary>
        static readonly Lazy<SessionManager> lazy =
        new Lazy<SessionManager>(() => new SessionManager());

        public static SessionManager Instance { get { return lazy.Value; } }
        private Stopwatch StopWatch = new Stopwatch();

        SessionManager()
        {
            SessionDuration = TimeSpan.FromSeconds(30);
        }

        public TimeSpan SessionDuration;

        public void EndTrackSession()
        {
            if (StopWatch.IsRunning)
            {
                StopWatch.Stop();
            }
        }

        public void ExtendSession()
        {
            if (StopWatch.IsRunning)
            {
                StopWatch.Restart();
            }
        }

        public void StartTrackSessionAsync()
        {
            if (!StopWatch.IsRunning)
            {
                StopWatch.Restart();
            }

            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 2), () =>
            {
                if (StopWatch.IsRunning && StopWatch.Elapsed.Seconds >= SessionDuration.Seconds)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        //Do something
                        if(RuntimeSettings.UploadKeeper == null)
                        {
                            DependencyService.Get<IToaster>().ShortAlert($"Synchronizacja plików w tle nie została odpowiednio uruchomiona.. Należy zrestartować aplikację..");
                        }
                        else
                        {
                            if (!RuntimeSettings.UploadKeeper.IsWorking)
                            {
                                await RuntimeSettings.UploadKeeper.RestoreUploadQueue();
                                if (RuntimeSettings.UploadKeeper.Items.Any())
                                {
                                    DependencyService.Get<IToaster>().ShortAlert($"Synchronizuje pliki..");
                                    await RuntimeSettings.UploadKeeper.Upload();
                                }
                                
                            }
                        }
                        if (RuntimeSettings.SyncKeeper == null)
                        {
                            DependencyService.Get<IToaster>().ShortAlert($"Synchronizacja danych offline w tle nie została odpowiednio uruchomiona.. Należy zrestartować aplikację..");
                        }
                        else
                        {
                            if (!RuntimeSettings.SyncKeeper.IsWorking)
                            {
                                await RuntimeSettings.SyncKeeper.RestoreSyncQueue();
                                await RuntimeSettings.SyncKeeper.Sync();
                            }
                        }



                    });

                    StopWatch.Stop();
                    StartTrackSessionAsync();
                }

                return true;
            });

        }


    }
}
