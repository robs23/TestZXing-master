using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Models;

namespace TestZXing.Interfaces
{
    public interface IFileHandler
    {
        string GetImageGalleryPath();
        string GetVideoGalleryPath();
    }
}
