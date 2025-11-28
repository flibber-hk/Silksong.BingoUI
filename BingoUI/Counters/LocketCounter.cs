using MonoDetour.HookGen;
using System;
using System.Linq;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(CollectableItemManager))]
public class LocketCounter(float x, float y, string spriteName) : AbstractCounter(x, y, spriteName)
{
    private const string ItemName = "Crest Socket Unlocker";

    public override string GetText()
    {
        int ownedLockets = PlayerData.instance.Collectables.GetData(ItemName).Amount;
        int spentLockets = 0;

        int hunterMax = 0;
        int hunterUnlocked = 0;

        foreach ((string key, ToolCrestsData.Data crestData) in PlayerData.instance.ToolEquips.Enumerate())
        {
            // For regular crests, all slots start unlocked except locked ones
            int locked = crestData.Slots.Where(x => !x.IsUnlocked).Count();
            int unlocked = crestData.Slots.Where(x => x.IsUnlocked).Count();

            switch (key)
            {
                case "Reaper":
                    spentLockets += 3 - locked;
                    continue;
                case "Warrior":
                    spentLockets += 2 - locked;
                    continue;
                case "Wanderer":
                    spentLockets += 3 - locked;
                    continue;
                case "Toolmaster":
                    spentLockets += 4 - locked;
                    continue;
                case "Witch":
                    spentLockets += 3 - locked;
                    continue;
                case "Spell":
                    spentLockets += 2 - locked;
                    continue;
                case "Cursed":
                    continue;
            }

            // For hunter crests, all slots start locked
            if (key.StartsWith("Hunter"))
            {
                int idx;
                if (key.Contains('v'))
                {
                    idx = Convert.ToInt32(key.Split("_v").Last());
                }
                else
                {
                    idx = 1;
                }

                if (idx > hunterMax)
                {
                    hunterMax = idx;
                    hunterUnlocked = unlocked;
                }
            }
        }

        spentLockets += hunterUnlocked;

        return (ownedLockets + spentLockets).ToString();
    }

    public override void Hook()
    {
        Md.CollectableItemManager.AffectItemData.Postfix(RecordGainedLocket);
    }

    private void RecordGainedLocket(CollectableItemManager self, ref string itemName, ref CollectableItemManager.ItemAffectingDelegate affector)
    {
        if (itemName == ItemName) UpdateTextNextFrame();
    }
}
