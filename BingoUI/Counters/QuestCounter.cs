using MonoDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingoUI.Counters;

[MonoDetourTargets(typeof(FullQuestBase))]
public class QuestCounter(float x, float y, string spriteName, string questTypeKeyPrefix) : AbstractCounter(x, y, spriteName)
{
    public override void Hook()
    {
        Md.FullQuestBase.SilentlyComplete.Prefix(OnQuestComplete);
        Md.FullQuestBase.TryEndQuest.Prefix(OnQuestComplete);
    }

    private void OnQuestComplete(
        FullQuestBase self, ref Action afterPrompt, ref bool consumeCurrency, ref bool forceEnd, ref bool showPrompt)
    {
        OnQuestComplete(self);
    }

    public bool Matches(FullQuestBase fq) => fq.QuestType.displayName.Key.StartsWith(questTypeKeyPrefix);

    private void OnQuestComplete(FullQuestBase self)
    {
        if (!this.Matches(self)) return;

        if (!self.Completion.WasEverCompleted)
        {
            UpdateTextNextFrame();
        }
    }

    public override string GetText()
    {
        return QuestManager
            .GetAllFullQuests()
            .Where(this.Matches)
            .Where(fq => fq.Completion.WasEverCompleted)
            .Count()
            .ToString();
    }

}
