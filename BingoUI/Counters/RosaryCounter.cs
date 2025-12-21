using PrepatcherPlugin;

namespace BingoUI.Counters;

public class RosaryCounter(string spriteName) : AbstractCounter(spriteName)
{
    public override void SetupHooks()
    {
        PlayerDataVariableEvents.OnSetInt += OnSetPdInt;
    }

    private int OnSetPdInt(PlayerData pd, string fieldName, int current)
    {
        if (fieldName == nameof(PlayerData.geo))
        {
            UpdateTextNextFrame();
        }

        return current;
    }

    public override string GetText()
    {
        // There isn't space for rosaries + spent
        return $"({CurrencyTracker.GetCurrencySpent(CurrencyType.Money)})";
    }
}
