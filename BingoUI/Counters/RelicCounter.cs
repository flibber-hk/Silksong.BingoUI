using MonoDetour.HookGen;
using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(CollectableRelicManager))]
public class RelicCounter(string spriteName, RelicTypes relicTypes, bool showDistinct) : AbstractCounter(spriteName)
{
    public RelicTypes CountedRelicTypes { get; set; } = relicTypes;
    public bool ShowDistinct { get; set; } = showDistinct;

    public static RelicTypes TypeForRelic(CollectableRelic relic)
    {
        return relic.RelicType.typeName.Key switch
        {
            "INV_NAME_R_BONE_RECORD" => RelicTypes.BoneScroll,
            "INV_NAME_R_SEAL_CHIT" => RelicTypes.ChoralCommandment,
            "INV_NAME_R_WEAVER_RECORD" => RelicTypes.RuneHarp,
            "INV_NAME_R_WEAVER_TOTEM" => RelicTypes.WeaverEffigy,
            "INV_NAME_R_ANCIENT_EGG" => RelicTypes.ArcaneEgg,
            "INV_NAME_R_PSALM_CYL" => RelicTypes.PsalmCylinder,
            "INV_NAME_R_PSALM_CYL_MELODY" => RelicTypes.SacredCylinder,

            _ => RelicTypes.Unknown
        };
    }

    public bool IsCounted(CollectableRelic relic)
    {
        return CountedRelicTypes.HasFlag(TypeForRelic(relic));
    }

    public IEnumerable<CollectableRelic> CountedRelics()
    {
        return CollectableRelicManager
            .GetAllRelics()
            .Where(relic => IsCounted(relic) && CollectableRelicManager.GetRelicData(relic).IsCollected);
    }

    public int DistinctRelicTypesCollected()
    {
        HashSet<RelicTypes> typesCollected = new();

        foreach (CollectableRelic relic in CountedRelics())
        {
            typesCollected.Add(TypeForRelic(relic));
        }

        return typesCollected.Count;
    }

    public override void SetupHooks()
    {
        Md.CollectableRelicManager.SetRelicData.Postfix(OnRelicCollected);
    }

    private void OnRelicCollected(ref CollectableRelic relic, ref CollectableRelicsData.Data data)
    {
        if (IsCounted(relic))
        {
            UpdateTextNextFrame();
        }
    }

    public override string GetText()
    {
        string collected = CountedRelics()
            .Count()
            .ToString();

        if (!ShowDistinct) return collected;

        string distinct = DistinctRelicTypesCollected().ToString();

        return $"{collected}({distinct})";
    }
}
