using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Models;

namespace TestZXing.Interfaces
{
    public interface IFileHandler
    {
        void OpenApk(string filepath);
        string GetImageGalleryPath();
        string GetVideoGalleryPath();
    }
}
