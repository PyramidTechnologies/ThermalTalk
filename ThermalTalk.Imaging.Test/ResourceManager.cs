using System;
using System.IO;
using SkiaSharp;

namespace ThermalTalk.Imaging.Test
{
    public static class ResourceManager
    {
        private static readonly string DirectoryPath;

        static ResourceManager()
        {
            var assembly = typeof(ResourceManager).Assembly;
            var assemblyName = assembly.GetName().Name;
            var projectDirectory = new DirectoryInfo(assembly.Location);

            while (projectDirectory.Name != assemblyName)
            {
                if (projectDirectory.Parent != null)
                    projectDirectory = projectDirectory.Parent;
                else
                    throw new Exception("Resources directory was not found.");
            }

            DirectoryPath = Path.Combine(projectDirectory.FullName, "Resources");
        }

        public static SKBitmap Load(string name, SKColorType? colorType = null)
        {
            var path = Path.Combine(DirectoryPath, name);
            var bitmap = SKBitmap.Decode(path);

            if (colorType == null || colorType == bitmap.ColorType)
                return bitmap;

            var convertedBitmap = bitmap.Copy(colorType.Value);
            bitmap.Dispose();
            return convertedBitmap;
        }
    }
}