using System;

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
    }
}