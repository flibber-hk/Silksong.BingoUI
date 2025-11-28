using PrepatcherPlugin;

namespace BingoUI.Counters;

internal class MapCounter(float x, float y, string spriteName) : AbstractCounter(x, y, spriteName)
{
    public override string GetText()
    {
        return PlayerData.instance.MapCount.ToString();
    }

    public override void Hook()
    {
        PlayerDataVariableEvents<bool>.OnSetVariable += OnSetMapBool;
    }

    private bool OnSetMapBool(PlayerData pd, string fieldName, bool current)
    {
        if (current && (fieldName.StartsWith("Has") && fieldName.EndsWith("Map")))
        {
            UpdateTextNextFrame();
        }

        return current;
    }
}
