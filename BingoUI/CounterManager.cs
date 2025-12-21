using System.Collections.Generic;
using BingoUI.Counters;
using ToolFlags = ToolItemManager.OwnToolsCheckFlags;

namespace BingoUI;

public class CounterManager
{
    public static string GetDisplayName(string spriteKey)
    {
        return spriteKey switch
        {
            "flea" => "Fleas",
            "maps" => "Maps",
            "lockets" => "Lockets",

            "alltools" => "All Tools",
            "redtools" => "Red Tools",
            "yellowtools" => "Yellow Tools",
            "bluetools" => "Blue Tools",

            "allquest" => "All Quests",
            "deliveryquest" => "Delivery Quests",
            "donationquest" => "Donation Quests",
            "huntquest" => "Hunt Quests",
            "gatherquest" => "Gather Quests",
            "wayfarerquest" => "Wayfarer Quests",

            "shardbundles" => "Shard Bundles",
            "beastshards" => "Beast Shards",
            "strings" => "Rosary Strings",
            "necklaces" => "Rosary Necklaces",

            "mapper" => "Shakra Scenes",

            "silkeater" => "Silkeaters",
            "bonescroll" => "Bone Scrolls",
            "choralcommandment" => "Choral Commandments",
            "craftmetal" => "Craftmetal",
            "psalmcylinder" => "Psalm Cylinders",
            "relics" => "Relics",
            "runeharp" => "Rune Harps",
            "weavereffigy" => "Weaver Effigies",
            
            "rosaries" => "Spent Rosaries",


            _ => spriteKey
        };
    }

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
            // TODO - deliveries

            new PickedConsumableCounter("shardbundles", ["Shard Pouch"]),
            new PickedConsumableCounter("beastshards", ["Great Shard"]),
            new PickedConsumableCounter("strings", ["Rosary_Set_Small", "Rosary_Set_Frayed"]),
            new PickedConsumableCounter("necklaces", ["Rosary_Set_Huge_White", "Rosary_Set_Medium", "Rosary_Set_Large"]),

            new ShakraLocationsCounter("mapper"),

            new RelicCounter("relics", RelicTypes.AnyRelic, true),
            new RelicCounter("bonescroll", RelicTypes.BoneScroll, false),
            new RelicCounter("choralcommandment", RelicTypes.ChoralCommandment, false),
            new RelicCounter("runeharp", RelicTypes.RuneHarp, false),
            new RelicCounter("weavereffigy", RelicTypes.WeaverEffigy, false),
            new RelicCounter("psalmcylinder", RelicTypes.AnyCylinder, false),

            // TODO - Craftmetal
            // TODO - Silkeaters

            // For people who want to show the rosary counter in the UI
            new RosaryCounter("rosaries")
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
