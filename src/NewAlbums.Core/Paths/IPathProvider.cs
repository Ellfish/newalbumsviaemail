﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Paths
{
    public interface IPathProvider
    {
        string GetAbsoluteEmailsFolderPath();
        
        string GetAbsoluteUrl(string relativeWebFilePath);
    }
}
