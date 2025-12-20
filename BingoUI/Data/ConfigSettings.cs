using BepInEx.Configuration;

namespace BingoUI.Data;

public static class ConfigSettings
{
    public static ConfigEntry<bool>? ShowSpentRosariesInHud;
    public static ConfigEntry<bool>? ShowSpentRosariesInInventory;

    public static ConfigEntry<DisplayMode>? CounterDisplayMode;

    public static bool AlwaysDisplayCounters => CounterDisplayMode?.Value == DisplayMode.AlwaysDisplay;
    public static bool NeverDisplayCounters => CounterDisplayMode?.Value == DisplayMode.NeverDisplay;

    public static void Setup(ConfigFile config)
    {
        ShowSpentRosariesInHud = config.Bind("Currency", nameof(ShowSpentRosariesInHud), true, "Show spent rosaries in HUD");
        ShowSpentRosariesInInventory = config.Bind("Currency", nameof(ShowSpentRosariesInInventory), true, "Show spent rosaries in inventory");
        CounterDisplayMode = config.Bind("Counters", nameof(CounterDisplayMode), DisplayMode.Default, "Counter display mode [currently forced hidden]");
    }
}
