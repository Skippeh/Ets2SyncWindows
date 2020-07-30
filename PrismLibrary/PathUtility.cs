using System;
using System.IO;

namespace PrismLibrary
{
    internal static class PathUtility
    {
        public static string GetGameDocumentsFolderName(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Ets2: return "Euro Truck Simulator 2";
                case GameType.Ats: return "American Truck Simulator";
            }

            throw new NotImplementedException("Unknown game type");
        }
        
        /// <summary>
        /// Returns true if filePath is contained within rootPath.
        /// </summary>
        public static bool IsPathContainedIn(string filePath, string rootPath)
        {
            // Set file paths to their full rooted path
            filePath = Path.GetFullPath(filePath);
            rootPath = Path.GetFullPath(rootPath);

            // Normalize separators
            filePath = filePath.Replace("\\", "/");
            rootPath = rootPath.Replace("\\", "/");

            // Ignore case because we're only targeting windows (would have to change this if that ever changes)
            return filePath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}