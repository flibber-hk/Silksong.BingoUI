using BingoUI.Data;
using MonoDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters;

/// <summary>
/// Counter representing the number of a type of consumable collected, where buying from a
/// repeatable shop is excluded from the count.
/// 
/// Shows X(Y), where X is #currently owned and Y is #ever picked up
/// </summary>
[MonoDetourTargets(typeof(CollectableItemManager))]
[MonoDetourTargets(typeof(CollectableItemPickup))]
internal class PickedConsumableCounter(string spriteName, HashSet<string> ItemNames) : AbstractCounter(spriteName)
{

    public override string GetText()
    {
        int shinyPicked = SaveData.Instance.PickedShinies.GetTotalAmount(ItemNames);
        int owned = ItemNames.Select(x => PlayerData.instance.Collectables.GetData(x).Amount).Sum();
        return $"{owned}({shinyPicked})";
    }
    
    public override void SetupHooks()
    {
        Md.CollectableItemPickup.DoPickupAction.Postfix(RecordPickedShiny);
        Md.CollectableItemManager.InternalAddItem.Postfix(RecordChangedItemCount);
        Md.CollectableItemManager.InternalRemoveItem.Postfix(RecordChangedItemCount);
    }

    private void RecordPickedShiny(CollectableItemPickup self, ref bool breakIfAtMax, ref bool returnValue)
    {
        if (!ItemNames.Contains(self.Item.name))
        {
            return;
        }

        SaveData.Instance.PickedShinies.Increment(self.Item.name);
    }

    private void RecordChangedItemCount(CollectableItemManager self, ref CollectableItem item, ref int amount)
    {
        if (!ItemNames.Contains(item.name)) return;
        UpdateTextNextFrame();
    }
}
