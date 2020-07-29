using PrismLibrary;

namespace Ets2SyncWindows.Data
{
    public static class GameData
    {
        public static readonly Game[] Games =
        {
            new Game(1, "Euro Truck Simulator 2", GameType.Ets2,

                MapDlc.GoingEast |
                MapDlc.Scandinavia |
                MapDlc.VivaLaFrance |
                MapDlc.Italy |
                MapDlc.BeyondTheBalticSea |
                MapDlc.RoadToTheBlackSea
                ,
                CargoDlc.PowerCargo |
                CargoDlc.HeavyCargo |
                CargoDlc.SpecialTransport
                ,
                TrailerDlc.Schwarzmuller |
                TrailerDlc.Krone
            ) { UiSortOrder = 0 },

            new Game(2, "American Truck Simulator", GameType.Ats,
                MapDlc.Nevada |
                MapDlc.Arizona |
                MapDlc.NewMexico |
                MapDlc.Oregon |
                MapDlc.Washington |
                MapDlc.Utah
                ,
                CargoDlc.HeavyCargo |
                CargoDlc.SpecialTransport
                ,
                0 /* no trailer dlcs */) { UiSortOrder = 2 },
            new Game(3, "Euro Truck Simulator 2 (ProMods)", GameType.Ets2,
                0 /* no map dlcs */,
                CargoDlc.PowerCargo |
                CargoDlc.HeavyCargo |
                CargoDlc.SpecialTransport
                ,
                TrailerDlc.Schwarzmuller |
                TrailerDlc.Krone
            ) { UiSortOrder = 1 }
        };
    }
}