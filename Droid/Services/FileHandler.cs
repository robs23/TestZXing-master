using System;
using System.Collections.Generic;
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

[assembly: Xamarin.Forms.Dependency(typeof(FileHandler))]
namespace TestZXing.Droid.Services
{
    public class FileHandler : IFileHandler
    {
        public string GetImageGalleryPath()
        {
            return Android.OS.Environment.DirectoryPictures;
        }
    }
}