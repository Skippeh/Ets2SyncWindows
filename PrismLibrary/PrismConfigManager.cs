using System;
using System.IO;
using System.Threading.Tasks;
using PrismLibrary.Exceptions;

namespace PrismLibrary
{
    public static class PrismConfigManager
    {
        /// <summary>
        /// Tries to load config from the current users documents folder.
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns>
        /// First tuple value indicates whether the config was loaded successfully.
        /// The second value is the config itself which is null if the first tuple value is false.
        /// </returns>
        public static (bool, PrismConfig) LoadGameConfig(GameType gameType)
        {
            string configPath = GetConfigFilePath(gameType);

            if (!File.Exists(configPath))
                return (false, null);
            
            using var fileStream = File.OpenRead(configPath);
            return LoadGameConfig(fileStream);
        }
        
        public static (bool, PrismConfig) LoadGameConfig(FileStream fileStream)
        {
            try
            {
                var result = PrismConfig.LoadConfig(fileStream);
                return (true, result);
            }
            catch (FileNotFoundException)
            {
                return (false, null);
            }
            catch (ConfigFormatException)
            {
                return (false, null);
            }
        }

        public static string GetConfigFilePath(GameType gameType)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gameName = PathUtility.GetGameDocumentsFolderName(gameType);
            string finalPath = Path.Combine(documentsPath, gameName, "config.cfg");

            return finalPath;
        }
    }
}