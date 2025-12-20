using BingoUI.Data;
using MonoDetour;
using MonoDetour.HookGen;
using System;
using TeamCherry.Localization;

namespace BingoUI;


[MonoDetourTargets(typeof(CurrencyCounterBase))]
[MonoDetourTargets(typeof(CurrencyCounter))]
public static class CurrencyTracker
{
    private static readonly MonoDetourManager currencyMgr = new("BingoUI.Currency");

    internal static void Hook()
    {
        Md.CurrencyCounterBase.LateUpdate.Postfix(ModifyCurrencyText, manager: currencyMgr);
        Md.CurrencyCounter.Take.Postfix(RecordSpentCurrency, manager: currencyMgr);
        Md.CurrencyCounter.Awake.Postfix(SetCurrencyTextSize, manager: currencyMgr);
    }

    private static void SetCurrencyTextSize(CurrencyCounter self)
    {
        if (self.currencyType != CurrencyType.Money) return;  // Could count shards too...
        if (self.geoTextMesh.tmpText is not TMProOld.TMP_Text text) return;
        text.fontSize *= 0.6f;
        self.initialAddTextX += 0.7f;
        self.initialSubTextX += 0.7f;
    }

    private static void RecordSpentCurrency(ref int amount, ref CurrencyType type)
    {
        int current = GetCurrencySpent(type);
        current += amount;
        SaveData.Instance.SpentCurrency[type] = current;
    }

    private static int GetCurrencySpent(CurrencyType type)
    {
        return SaveData.Instance.SpentCurrency.TryGetValue(type, out int spent) ? spent : 0;
    }

    private static void ModifyCurrencyText(CurrencyCounterBase self)
    {
        if (self is not CurrencyCounter ctr) return;
        if (ctr.currencyType != CurrencyType.Money) return;

        string spentGeoString;
        try
        {
            spentGeoString = string.Format(
                Language.Get("SPENT_GEO_FMT", $"Mods.{BingoUIPlugin.Id}"),
                GetCurrencySpent(ctr.CounterType)
            );
        }
        catch (Exception)
        {
            // If a localizer didn't put {0} in the string I don't want to blow up the mod
            spentGeoString = $"{GetCurrencySpent(ctr.CounterType)} spent";
        }
        
        ctr.geoTextMesh.Text = $"{self.counterCurrent}\n({spentGeoString})";
    }
}