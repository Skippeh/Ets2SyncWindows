using System.Collections.Generic;
using System.Linq;
using PrismLibrary;
using PrismSyncLibrary;

namespace Ets2SyncWindows.Data
{
    public class Game
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public MapDlc MapDlcs { get; set; }
        public CargoDlc CargoDlcs { get; set; }
        public TrailerDlc TrailerDlcs { get; set; }
        public int UiSortOrder { get; set; }
        public GameType Type { get; set; }
        public GameModType ModType { get; set; }

        public Game(int index, string name, GameType gameType, GameModType modType, MapDlc mapDlcs = 0, CargoDlc cargoDlcs = 0, TrailerDlc trailerDlcs = 0)
        {
            Index = index;
            Name = name;
            MapDlcs = mapDlcs;
            CargoDlcs = cargoDlcs;
            TrailerDlcs = trailerDlcs;
            Type = gameType;
            ModType = modType;
        }
    }
}