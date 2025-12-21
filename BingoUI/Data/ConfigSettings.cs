using BepInEx.Configuration;

namespace BingoUI.Data;

public static class ConfigSettings
{
    public static ConfigEntry<bool>? ShowSpentRosariesInHud;
    public static ConfigEntry<bool>? ShowSpentRosariesInInventory;

    // TODO - should fix what happens if this setting changes
    public static ConfigEntry<DisplayMode>? CounterDisplayMode;

    public static bool AlwaysDisplayCounters => false; // CounterDisplayMode?.Value == DisplayMode.AlwaysDisplay;
    public static bool NeverDisplayCounters => true; // CounterDisplayMode?.Value == DisplayMode.NeverDisplay;

    public static void Setup(ConfigFile config)
    {
        ShowSpentRosariesInHud = config.Bind("Currency", nameof(ShowSpentRosariesInHud), true, "Show spent rosaries in HUD");
        ShowSpentRosariesInInventory = config.Bind("Currency", nameof(ShowSpentRosariesInInventory), true, "Show spent rosaries in inventory");
        CounterDisplayMode = config.Bind("Counters", nameof(CounterDisplayMode), DisplayMode.Default, "Counter display mode [currently forced hidden]");
    }
}
