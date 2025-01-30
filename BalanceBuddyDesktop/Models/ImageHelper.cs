using System;
using System.IO;
using Avalonia.Platform;
using Avalonia.Media.Imaging;

public static class ImageHelper
{
    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
}
