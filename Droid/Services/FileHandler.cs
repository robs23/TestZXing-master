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

        public void OpenFile(File f)
        {
            Java.IO.File file = new Java.IO.File(f.Link);
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "image/jpeg");
            Xamarin.Forms.Forms.Context.StartActivity(intent);
        }

    }
}