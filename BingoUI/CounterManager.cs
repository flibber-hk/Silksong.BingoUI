using System.Collections.Generic;
using System.Linq;
using BingoUI.Counters;
using ToolFlags = ToolItemManager.OwnToolsCheckFlags;

namespace BingoUI;

public class CounterManager
{
    public List<AbstractCounter> Counters { get; }

    public Dictionary<string, AbstractCounter> CounterLookup { get; }

    public CounterManager()
    {
        Counters =
        [
            // new CounterType(default x position, default y position, sprite name, any other parameters)
            new FleaCounter(14f / 15f, 0.01f, "flea"),
            new MapCounter(13f / 15f, 0.01f, "maps"),
            new LocketCounter(12f / 15f, 0.01f, "lockets"),

            new ToolTypeCounter(11f / 15f, 0.01f, "alltools", ToolFlags.Red | ToolFlags.Yellow | ToolFlags.Blue),
            new ToolTypeCounter(10f / 15f, 0.01f, "redtools", ToolFlags.Red),
            new ToolTypeCounter(9f / 15f, 0.01f, "yellowtools", ToolFlags.Yellow),
            new ToolTypeCounter(8f / 15f, 0.01f, "bluetools", ToolFlags.Blue),
            // Only count non-main quests
            new QuestCounter(7f / 15f, 0.01f, "allquest", "TYPE_"),
            new QuestCounter(6f / 15f, 0.01f, "donationquest", "TYPE_DONATE_"),
            new QuestCounter(5f / 15f, 0.01f, "huntquest", "TYPE_HUNT_"),
            new QuestCounter(4f / 15f, 0.01f, "gatherquest", "TYPE_GATHER_"),
            new QuestCounter(3f / 15f, 0.01f, "wayfarerquest", "TYPE_WAYFARER_"),

            new PickedConsumableCounter(2f / 15f, 0.01f, "shardbundles", ["Shard Pouch"]),
            new PickedConsumableCounter(1f / 15f, 0.01f, "beastshards", ["Great Shard"]),
            new PickedConsumableCounter(0f / 15f, 0.01f, "strings", ["Rosary_Set_Small", "Rosary_Set_Frayed"]),
            new PickedConsumableCounter(14f / 15f, 0.12f, "necklaces", ["Rosary_Set_Huge_White", "Rosary_Set_Medium", "Rosary_Set_Large"]),

        ];
        CounterLookup = Counters.ToDictionary(v => v.SpriteName, v => v);

        foreach (AbstractCounter counter in Counters)
        {
            counter.Hook();
            counter.UpdateText();
        }
    }
}
