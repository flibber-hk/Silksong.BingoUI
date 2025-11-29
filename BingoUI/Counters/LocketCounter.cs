using MonoDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(CollectableItemManager))]
public class LocketCounter(string spriteName) : AbstractCounter(spriteName)
{
    private const string ItemName = "Crest Socket Unlocker";

    public override string GetText()
    {
        int unlockedSlots = 0;
        foreach (ToolCrest crest in ToolItemManager.GetAllCrests())
        {
            if (
              crest.IsHidden || !crest.IsUnlocked
              || crest.IsUpgradedVersionUnlocked
            )
            {
                continue;
            }

            ToolCrest.SlotInfo[] initialSlots = crest.Slots;
            List<ToolCrestsData.SlotData> savedSlots = crest.SaveData.Slots;

            for (int i = 0; i < initialSlots.Length; i++)
            {
                if (
                  initialSlots[i].IsLocked
                  && (savedSlots != null && i < savedSlots.Count && savedSlots[i].IsUnlocked)
                )
                {
                    unlockedSlots++;
                }
            }
        }

        int ownedLockets = PlayerData.instance.Collectables.GetData(ItemName).Amount;
        int totalLockets = ownedLockets + unlockedSlots;

        return $"{ownedLockets}({totalLockets})";
    }

    public override void SetupHooks()
    {
        Md.CollectableItemManager.AffectItemData.Postfix(RecordGainedLocket);
    }

    private void RecordGainedLocket(CollectableItemManager self, ref string itemName, ref CollectableItemManager.ItemAffectingDelegate affector)
    {
        if (itemName == ItemName) UpdateTextNextFrame();
    }
}
