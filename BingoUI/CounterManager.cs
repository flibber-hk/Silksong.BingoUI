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
        ];
        CounterLookup = Counters.ToDictionary(v => v.SpriteName, v => v);

        foreach (AbstractCounter counter in Counters)
        {
            counter.Hook();
            counter.UpdateText();
        }
    }
}
