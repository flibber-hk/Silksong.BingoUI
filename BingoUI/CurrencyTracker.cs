using BepInEx.Logging;
using BingoUI.Components;
using BingoUI.Data;
using Md.HeroController;
using MonoDetour;
using MonoDetour.HookGen;
using Silksong.UnityHelper.Extensions;
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
        Md.CurrencyCounter.Take.Postfix(RecordSpentCurrency, manager: currencyMgr);
        Md.CurrencyCounter.Awake.Postfix(AddCurrencyTrackers, manager: currencyMgr);
    }

    private static void AddCurrencyTrackers(CurrencyCounter self)
    {
        if (self.currencyType != CurrencyType.Money) return;  // Could count shards too...

        self.gameObject.AddComponent<CurrencyCounterModifier>();
    }

    private static void RecordSpentCurrency(ref int amount, ref CurrencyType type)
    {
        int current = GetCurrencySpent(type);
        current += amount;
        SaveData.Instance.SpentCurrency[type] = current;
    }

    internal static int GetCurrencySpent(CurrencyType type)
    {
        return SaveData.Instance.SpentCurrency.TryGetValue(type, out int spent) ? spent : 0;
    }
}