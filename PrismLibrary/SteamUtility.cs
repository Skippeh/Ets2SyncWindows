using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PrismLibrary
{
    internal static class SteamUtility
    {
        public static string GetSteamInstallDirectory()
        {
            EnsurePlatformIsWindows();

            RegistryKey valveKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            object steamPath = valveKey?.GetValue("SteamPath");

            if (steamPath is string stringSteamPath)
                return stringSteamPath;

            return null;
        }

        /// <summary>
        /// Returns all cloud data file paths for each steam profile.
        /// Key = the steam accounts steamid3 identifier.
        /// Value = file path to the apps cloud data path.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAppIdCloudDataPath(ulong appId)
        {
            EnsurePlatformIsWindows();

            string steamPath = GetSteamInstallDirectory();
            var result = new Dictionary<string, string>();

            foreach (string directoryPath in Directory.EnumerateDirectories(Path.Combine(steamPath, "userdata")))
            {
                var appIdPath = Path.Combine(directoryPath, appId.ToString(), "remote");

                if (!Directory.Exists(appIdPath))
                    continue;

                string steamId3 = new DirectoryInfo(directoryPath).Name;
                result.Add(steamId3, appIdPath);
            }
            
            return result;
        }

        private static void EnsurePlatformIsWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("Non windows operating systems are not supported.");
        }
    }
}