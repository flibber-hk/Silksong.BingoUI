using System.Collections.Generic;
using System.Linq;
using BingoUI.Counters;

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
        ];
        CounterLookup = Counters.ToDictionary(v => v.SpriteName, v => v);

        foreach (AbstractCounter counter in Counters)
        {
            counter.Hook();
            counter.UpdateText();
        }
    }
}
