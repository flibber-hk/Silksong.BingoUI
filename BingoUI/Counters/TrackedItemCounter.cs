using BingoUI.Data;
using MonoDetour.HookGen;
using System;

namespace BingoUI.Counters;

/// <summary>
/// Counter that tracks the total number of an item and the number ever obtained.
/// </summary>
[MonoDetourTargets(typeof(CollectableItemManager))]
internal class TrackedItemCounter(string spriteName, string itemName) : AbstractCounter(spriteName)
{
    public string ItemName { get; set; } = itemName;

    public int CurrentlyHeld => PlayerData.instance.Collectables.GetData(ItemName).Amount;
    public int EverObtained => SaveData.Instance.ItemsEverObtained.TryGetValue(ItemName, out int value) ? value : 0;

    public override void SetupHooks()
    {
        Md.CollectableItemManager.AddItem.Postfix(OnAddItem);
        Md.CollectableItemManager.RemoveItem.Postfix(OnRemoveItem);
    }

    private void OnRemoveItem(ref CollectableItem item, ref int amount)
    {
        if (item.name != ItemName) return;

        UpdateTextNextFrame();
    }

    private void OnAddItem(ref CollectableItem item, ref int amount)
    {
        if (item.name != ItemName) return;

        SaveData.Instance.ItemsEverObtained[ItemName] = EverObtained + amount;

        UpdateTextNextFrame();
    }

    public override string GetText()
    {
        return $"{CurrentlyHeld}({EverObtained})";
    }
}
