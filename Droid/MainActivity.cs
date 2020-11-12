using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Plugin.CurrentActivity;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace TestZXing.Droid
{
    [Activity(Label = "JDE Scanner", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            //Distribute.ReleaseAvailable = OnReleaseAvailable;
            AppCenter.Start($"{Static.Secrets.AppCenterSecret}", typeof(Analytics), typeof(Crashes), typeof(Distribute));
                
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Android.Glide.Forms.Init(this);
            string localDbFileName = "JDE_Scan_db.db3";
            string localDbFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localDbPath = Path.Combine(localDbFolderPath, localDbFileName);
            InitializeNLog();
            LoadApplication(new App(localDbPath));
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void InitializeNLog()
        {
            Assembly assembly = this.GetType().Assembly;
            string assemblyName = assembly.GetName().Name;
            new Classes.LogService().Initialize(assembly, assemblyName);
        }
    }
}
