using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BingoUI.Counters;
using ToolFlags = ToolItemManager.OwnToolsCheckFlags;

namespace BingoUI;

public class CounterManager
{
    private static List<AbstractCounter> GenerateDefaultCounters()
    {
        return [
            // new CounterType(sprite name, any other parameters)
            new FleaCounter("flea"),
            new MapCounter("maps"),
            new LocketCounter("lockets"),

            new ToolTypeCounter("alltools", ToolFlags.Red | ToolFlags.Yellow | ToolFlags.Blue),
            new ToolTypeCounter("redtools", ToolFlags.Red),
            new ToolTypeCounter("yellowtools", ToolFlags.Yellow),
            new ToolTypeCounter("bluetools", ToolFlags.Blue),
            // Only count non-main quests
            new QuestCounter("allquest", "TYPE_"),
            new QuestCounter("donationquest", "TYPE_DONATE_"),
            new QuestCounter("huntquest", "TYPE_HUNT_"),
            new QuestCounter("gatherquest", "TYPE_GATHER_"),
            new QuestCounter("wayfarerquest", "TYPE_WAYFARER_"),

            new PickedConsumableCounter("shardbundles", ["Shard Pouch"]),
            new PickedConsumableCounter("beastshards", ["Great Shard"]),
            new PickedConsumableCounter("strings", ["Rosary_Set_Small", "Rosary_Set_Frayed"]),
            new PickedConsumableCounter("necklaces", ["Rosary_Set_Huge_White", "Rosary_Set_Medium", "Rosary_Set_Large"]),
        ];
    }

    public Dictionary<string, AbstractCounter> Counters { get; }

    public CounterManager()
    {

        Counters = [];

        foreach (AbstractCounter counter in GenerateDefaultCounters())
        {
            Counters[counter.SpriteName] = counter;
        }

        foreach (AbstractCounter counter in Counters.Values)
        {
            counter.SetupHooks();
            counter.UpdateText();
        }
    }
}
