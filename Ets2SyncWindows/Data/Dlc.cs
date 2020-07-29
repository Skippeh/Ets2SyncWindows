using System;
using System.ComponentModel;

namespace Ets2SyncWindows.Data
{
    public class IgnoreAttribute : Attribute
    {
    }
    
    [Flags]
    public enum MapDlc
    {
        [Ignore]
        None,
        
        // ETS2
        [Description("Going East")] GoingEast = 2,
        Scandinavia = 512,
        [Description("Viva la France")] VivaLaFrance = 4,
        Italy = 8,
        [Description("Beyond the Baltic Sea")] BeyondTheBalticSea = 64,
        [Description("Road to the Black Sea")] RoadToTheBlackSea = 1024,
        
        // ATS
        Nevada = 8192,
        Arizona = 4096,
        [Description("New Mexico")] NewMexico = 16384,
        Oregon = 32768,
        Washington = 262144,
        Utah = 131072
    }

    [Flags]
    public enum CargoDlc
    {
        [Ignore]
        None,
        
        [Description("Power Cargo")] PowerCargo = 16,
        [Description("Heavy Cargo")] HeavyCargo = 32,
        [Description("Special Transport")] SpecialTransport = 2048
    }

    [Flags]
    public enum TrailerDlc
    {
        [Ignore]
        None,
        
        Schwarzmuller = 256,
        Krone = 128
    }
}