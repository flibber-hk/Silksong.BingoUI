using BepInEx.Configuration;
using Silksong.ModMenu.Plugin;
using System.Collections.Generic;

namespace BingoUI.Data;

public static class ConfigSettings
{
    public static ConfigEntry<bool>? ShowSpentRosariesInHud;
    public static ConfigEntry<bool>? ShowSpentRosariesInInventory;

    // TODO - should fix what happens if this setting changes
    public static ConfigEntry<DisplayMode>? CounterDisplayMode;

    public static Dictionary<string, ConfigEntry<bool>>? CounterSettings;

    public static bool AlwaysDisplayCounters => CounterDisplayMode?.Value == DisplayMode.AlwaysDisplay;
    public static bool NeverDisplayCounters => CounterDisplayMode?.Value == DisplayMode.NeverDisplay;

    public static void Setup(ConfigFile config, CounterManager counterManager)
    {
        ShowSpentRosariesInHud = config.Bind("Currency", nameof(ShowSpentRosariesInHud), true, "Show spent rosaries in HUD");
        ShowSpentRosariesInInventory = config.Bind("Currency", nameof(ShowSpentRosariesInInventory), true, "Show spent rosaries in inventory");
        CounterDisplayMode = config.Bind(
            "Counters",
            nameof(CounterDisplayMode),
            DisplayMode.Default,
            new ConfigDescription(
                "When to display counters",
                null,
                MenuElementGenerators.CreateRightDescGenerator(true)
                )
            );

        CounterSettings = [];
        foreach (string spriteName in counterManager.Counters.Keys)
        {
            string counterDisplayName = CounterManager.GetDisplayName(spriteName);
            CounterSettings[spriteName] = config.Bind("Counters.Individual", counterDisplayName, spriteName != "rosaries", $"Show counter: {counterDisplayName}");
        }
    }
}
