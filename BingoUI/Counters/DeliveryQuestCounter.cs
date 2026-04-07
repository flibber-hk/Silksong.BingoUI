using BingoUI.Data;
using System;
using System.Linq;

namespace BingoUI.Counters;

public class DeliveryQuestCounter(string spriteName) : AbstractCounter(spriteName)
{
    public override void SetupHooks()
    {
        Md.FullQuestBase.TryEndQuest.Postfix(OnCompleteQuest);
    }

    private void OnCompleteQuest(
        FullQuestBase self,
        ref Action afterPrompt,
        ref bool consumeCurrency, 
        ref bool forceEnd, 
        ref bool showPrompt, 
        ref bool returnValue)
    {
        if (!returnValue) return;

        if (!self.QuestType.displayName.Key.StartsWith("TYPE_COURIER_")) return;

        string name = self.name;
        int current = SaveData.Instance.DeliveryQuestsCompleted.TryGetValue(name, out int val) ? val : 0;
        SaveData.Instance.DeliveryQuestsCompleted[name] = current + 1;
    }

    public override string GetText()
    {
        int total = SaveData.Instance.DeliveryQuestsCompleted.Values.Sum();
        int distinct = SaveData.Instance.DeliveryQuestsCompleted.Values.Where(x => x != 0).Count();

        return $"{total}{{{distinct}}}";
    }
}
