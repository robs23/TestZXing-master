﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Interfaces
{
    public interface IFileHandler
    {
        string GetImageGalleryPath();
        string GetVideoGalleryPath();
    }
}