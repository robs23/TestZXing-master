using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TestZXing.Droid.Services;
using TestZXing.Interfaces;
using File = TestZXing.Models.File;

[assembly: Xamarin.Forms.Dependency(typeof(FileHandler))]
namespace TestZXing.Droid.Services
{
    public class FileHandler : IFileHandler
    {
        public string GetImageGalleryPath()
        {

            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            using (var mediaStorageDir = new Java.IO.File(directory,string.Empty))
            {
                if (!mediaStorageDir.Exists())
                {
                    if (!mediaStorageDir.Mkdirs())
                        throw new IOException("Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");

                }

                return mediaStorageDir.Path;
            }
        }

        public string GetVideoGalleryPath()
        {

            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies);
            using (var mediaStorageDir = new Java.IO.File(directory, string.Empty))
            {
                if (!mediaStorageDir.Exists())
                {
                    if (!mediaStorageDir.Mkdirs())
                        throw new IOException("Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");

                }

                return mediaStorageDir.Path;
            }
        }

        public void OpenApk(string filepath)
        {
            Java.IO.File file = new Java.IO.File(filepath);
            Intent install = new Intent(Intent.ActionView);

            // Old Approach
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.N)
            {
                install.SetFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
                install.SetDataAndType(Android.Net.Uri.FromFile(file), "application/vnd.android.package-archive"); //mimeType
            }
            else
            {
                Android.Net.Uri apkURI = Android.Support.V4.Content.FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.ApplicationContext.PackageName + ".fileprovider", file);
                install.SetDataAndType(apkURI, "application/vnd.android.package-archive");
                install.AddFlags(ActivityFlags.NewTask);
                install.AddFlags(ActivityFlags.GrantReadUriPermission);
            }

            Android.App.Application.Context.StartActivity(install);
        }

    }
}