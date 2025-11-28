using MonoDetour.HookGen;

namespace BingoUI;


[MonoDetourTargets(typeof(CurrencyCounterBase))]
[MonoDetourTargets(typeof(CurrencyCounter))]
public static class CurrencyTracker
{
    internal static void Hook()
    {
        Md.CurrencyCounterBase.LateUpdate.Postfix(ModifyCurrencyText);
        Md.CurrencyCounter.Take.Postfix(RecordSpentCurrency);
        Md.CurrencyCounter.Awake.Postfix(SetCurrencyTextSize);
    }

    private static void SetCurrencyTextSize(CurrencyCounter self)
    {
        if (self.currencyType != CurrencyType.Money) return;  // Could count shards...
        if (self.geoTextMesh.tmpText is not TMProOld.TMP_Text text) return;
        text.fontSize *= 0.6f;
        self.initialAddTextX += 0.7f;
        self.initialSubTextX += 0.7f;
    }

    private static void RecordSpentCurrency(ref int amount, ref CurrencyType type)
    {
        int current = GetCurrencySpent(type);
        current += amount;
        SaveDataProxy.SpentCurrency[type] = current;
    }

    private static int GetCurrencySpent(CurrencyType type)
    {
        return SaveDataProxy.SpentCurrency.TryGetValue(type, out int spent) ? spent : 0;
    }

    private static void ModifyCurrencyText(CurrencyCounterBase self)
    {
        if (self is not CurrencyCounter ctr) return;
        if (ctr.currencyType != CurrencyType.Money) return;
        ctr.geoTextMesh.Text = $"{self.counterCurrent}\n({GetCurrencySpent(ctr.CounterType)} spent)";
    }
}