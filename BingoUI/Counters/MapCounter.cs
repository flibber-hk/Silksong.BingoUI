using PrepatcherPlugin;

namespace BingoUI.Counters;

internal class MapCounter(string spriteName) : AbstractCounter(spriteName)
{
    public override string GetText()
    {
        // Make sure mapBoolList is assigned
        var _ = PlayerData.instance.MapBools;
        return PlayerData.instance.MapCount.ToString();
    }

    public override void SetupHooks()
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
