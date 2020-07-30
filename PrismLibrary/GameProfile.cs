using System;

namespace PrismLibrary
{
    public class GameProfile
    {
        public string Name { get; set; }
        public string RootFilePath { get; set; }
        public ProfileType Type { get; set; }
        public GameSave[] Saves { get; set; }
        public DateTime LastSaveTime { get; set; }

        public GameProfile(ProfileType type)
        {
            Type = type;
        }
    }
}