using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PrismLibrary
{
    public class GameProfile
    {
        public string Name { get; set; }
        public string RootFilePath { get; set; }
        public ProfileType Type { get; set; }
        public ObservableCollection<GameSave> Saves { get; set; }
        public DateTime LastSaveTime { get; set; }
        public GameType GameType { get; set; }

        public GameProfile(ProfileType type)
        {
            Type = type;
        }
    }
}