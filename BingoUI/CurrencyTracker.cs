using MonoDetour.HookGen;
using Silksong.GameObjectDump;
using Silksong.UnityHelper.Extensions;
using System;

namespace BingoUI;


[MonoDetourTargets(typeof(CurrencyCounterBase))]
[MonoDetourTargets(typeof(CurrencyCounter))]
public static class CurrencyTracker
{
    internal static void Hook()
    {
        Md.CurrencyCounterBase.LateUpdate.Postfix(ModifyCurrencyText);
        Md.CurrencyCounter.Take.Postfix(RecordSpentCurrency);


        Md.CurrencyCounter.Awake.Postfix(DoDump);

        // On.GeoCounter.Update += CurrencyTracker.UpdateGeoText;
        // On.GeoCounter.TakeGeo += CurrencyTracker.CheckGeoSpent;
    }

    private static void DoDump(CurrencyCounter self)
    {
        string name = self.currencyType.ToString();
        self.InvokeAfterFrames(() => self.gameObject.Dump(dumpOptions: new() { DumpFullComponent = c => c is not PlayMakerFSM}), 10);
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
        ctr.geoTextMesh.Text = $"{self.counterCurrent} ({GetCurrencySpent(ctr.CounterType)} spent)";
    }

    /*
    private static void CheckGeoSpent(On.GeoCounter.orig_TakeGeo orig, GeoCounter self, int geo)
    {
        orig(self, geo);

        if (GameManager.instance.GetSceneNameString() == ItemChanger.SceneNames.Fungus3_35 && PlayerData.instance.GetBool(nameof(PlayerData.bankerAccountPurchased)))
        {
            return;
        }

        BingoUI.LS.spentGeo += geo;
    }

    private static void UpdateGeoText(On.GeoCounter.orig_Update orig, GeoCounter self)
    {
        orig(self);
        if (BingoUI.GS.showSpentGeo)
        {
            self.geoTextMesh.text = $"{GetGeoCounterCurrent(self)} ({BingoUI.LS.spentGeo} spent)";
        }
    }
    */
}