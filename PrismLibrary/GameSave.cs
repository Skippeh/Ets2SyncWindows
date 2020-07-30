using System;

namespace PrismLibrary
{
    public class GameSave
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public DateTime SaveTime { get; set; }
        public GameSaveType SaveType { get; set; }
    }
}