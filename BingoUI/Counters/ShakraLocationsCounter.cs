using BingoUI.Data;
using MonoDetour.HookGen;
using System;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(PlayMakerNPC))]
public class ShakraLocationsCounter(string spriteName) : AbstractCounter(spriteName)
{
    public override string GetText()
    {
        return SaveData.Instance.ShakraScenes.Count.ToString();
    }

    public override void SetupHooks()
    {
        Md.PlayMakerNPC.OnStartDialogue.Postfix(OnShakraTalk);
    }

    /// <summary>
    /// Check if the NPC is probably shakra
    /// </summary>
    private bool CheckShakra(PlayMakerNPC npc)
    {
        if (npc.gameObject.name.StartsWith("Mapper")) return true;

        return false;
    }

    private void OnShakraTalk(PlayMakerNPC self)
    {
        if (!CheckShakra(self)) return;

        string sceneName = GameManager.instance.sceneName;
        if (SaveData.Instance.ShakraScenes.Add(sceneName))
        {
            UpdateTextNextFrame();
        }
    }
}
