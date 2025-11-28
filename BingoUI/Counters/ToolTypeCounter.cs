using MonoDetour.HookGen;
using System;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(ToolItem))]
internal class ToolTypeCounter(float x, float y, string spriteName, ToolItemManager.OwnToolsCheckFlags flags) : AbstractCounter(x, y, spriteName)
{
    public override string GetText()
    {
        return ToolItemManager.GetOwnedToolsCount(flags).ToString();
    }

    public override void Hook()
    {
        Md.ToolItem.Unlock.Postfix(OnToolUnlock);
    }

    private void OnToolUnlock(ToolItem self, ref Action afterTutorialMsg, ref ToolItem.PopupFlags popupFlags)
    {
        switch (self.type)
        {
            case ToolItemType.Red when flags.HasFlag(ToolItemManager.OwnToolsCheckFlags.Red):
            case ToolItemType.Yellow when flags.HasFlag(ToolItemManager.OwnToolsCheckFlags.Yellow):
            case ToolItemType.Blue when flags.HasFlag(ToolItemManager.OwnToolsCheckFlags.Blue):
                UpdateTextNextFrame();
                break;
        }
    }
}
