using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            AppCenter.Start($"{Static.Secrets.AppCenterSecret}", typeof(Analytics), typeof(Crashes));
                
            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
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
    }
}
