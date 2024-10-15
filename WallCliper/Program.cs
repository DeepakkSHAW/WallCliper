using Microsoft.Win32;
using System;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace WallCliper
{
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
        UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        static public void SetWallpaper(String path)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 0.ToString()); // 2 is stretched
            key.SetValue(@"TileWallpaper", 0.ToString());

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        [STAThread]
        static void Main()
        {
            //Get System Info //
            var systemInfos = new List<string>();

            //var host = Dns.GetHostName();
            //var IP = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(i => i.IsIPv6LinkLocal == false);
            //var osVersion = Environment.OSVersion;
            //var Is64BitOS = Environment.Is64BitOperatingSystem;
            //var userName = Environment.UserName;
            //var domainName = Environment.UserDomainName;
            //var version = Environment.Version;

            systemInfos.Add($"HOST NAME: {Dns.GetHostName()}");
            systemInfos.Add($"IP ADDRESS: {Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(i => i.IsIPv6LinkLocal == false)}");
            systemInfos.Add($"OS Version: {Environment.OSVersion}");
            systemInfos.Add($"Is 64BitOS: {Environment.Is64BitOperatingSystem}");
            systemInfos.Add($"USER NAME: {Environment.UserName}");
            systemInfos.Add($"DOMAIN NAME: {Environment.UserDomainName}");
            systemInfos.Add($"VERSION: {Environment.Version}");

            // pic file path
            var path = AssemblyDirectory + "\\Pics";
            //Get random pic each time//
            var rndPic = new Random();
            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg")).ToList();
            var imgWallpaper = files[rndPic.Next(files.Count)];

            //write system info on image//
            var imageFilePathTarget = @"C:\temp\DK-wallpaper.jpg";
            WriteOnImage(imgWallpaper, imageFilePathTarget, systemInfos);

            // verify
            if (File.Exists(imageFilePathTarget))
            {
                SetWallpaper(imageFilePathTarget);
            }
        }

        static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        static void WriteOnImage(string imageFilePath, string newImageFileName, List<string> Infos)
        {
            var nextText = 50f;
            Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(imageFilePath);//load the image file

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                foreach (var text in Infos)
                {
                    using (System.Drawing.Font arialFont = new System.Drawing.Font("Arial", 12, FontStyle.Bold))
                    {
                        graphics.DrawString(text, arialFont, Brushes.YellowGreen, new PointF(1200f, nextText));
                        nextText = nextText + 20f;
                    }
                }
            }
            bitmap.Save(newImageFileName);//save the image file
        }
    }
}