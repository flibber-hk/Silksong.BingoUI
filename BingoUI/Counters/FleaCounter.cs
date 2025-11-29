using GlobalEnums;
using PrepatcherPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace BingoUI.Counters;

public class FleaCounter(string spriteName) : AbstractCounter(spriteName)
{
    private static readonly Dictionary<string, MapZone> FleaBools = new()
    {
        [nameof(PlayerData.SavedFlea_Ant_03)] = MapZone.HUNTERS_NEST,
        [nameof(PlayerData.SavedFlea_Belltown_04)] = MapZone.BELLTOWN,
        [nameof(PlayerData.SavedFlea_Bone_06)] = MapZone.PATH_OF_BONE,
        [nameof(PlayerData.SavedFlea_Bone_East_05)] = MapZone.DOCKS,
        [nameof(PlayerData.SavedFlea_Bone_East_10_Church)] = MapZone.WILDS,
        [nameof(PlayerData.SavedFlea_Bone_East_17b)] = MapZone.WILDS,
        [nameof(PlayerData.SavedFlea_Coral_24)] = MapZone.RED_CORAL_GORGE,
        [nameof(PlayerData.SavedFlea_Coral_35)] = MapZone.JUDGE_STEPS,
        [nameof(PlayerData.SavedFlea_Crawl_06)] = MapZone.CRAWLSPACE,
        [nameof(PlayerData.SavedFlea_Dock_03d)] = MapZone.DOCKS,
        [nameof(PlayerData.SavedFlea_Dock_16)] = MapZone.DOCKS,
        [nameof(PlayerData.SavedFlea_Dust_09)] = MapZone.SWAMP,
        [nameof(PlayerData.SavedFlea_Dust_12)] = MapZone.DUSTPENS,
        [nameof(PlayerData.SavedFlea_Greymoor_06)] = MapZone.GREYMOOR,
        [nameof(PlayerData.SavedFlea_Greymoor_15b)] = MapZone.GREYMOOR,
        [nameof(PlayerData.SavedFlea_Library_01)] = MapZone.LIBRARY,
        [nameof(PlayerData.SavedFlea_Library_09)] = MapZone.LIBRARY,
        [nameof(PlayerData.SavedFlea_Peak_05c)] = MapZone.PEAK,
        [nameof(PlayerData.SavedFlea_Shadow_10)] = MapZone.SWAMP,
        [nameof(PlayerData.SavedFlea_Shadow_28)] = MapZone.SWAMP,
        [nameof(PlayerData.SavedFlea_Shellwood_03)] = MapZone.SHELLWOOD_THICKET,
        [nameof(PlayerData.SavedFlea_Slab_06)] = MapZone.THE_SLAB,
        [nameof(PlayerData.SavedFlea_Slab_Cell)] = MapZone.THE_SLAB,
        [nameof(PlayerData.SavedFlea_Song_11)] = MapZone.CITY_OF_SONG,
        [nameof(PlayerData.SavedFlea_Song_14)] = MapZone.CITY_OF_SONG,
        [nameof(PlayerData.SavedFlea_Under_21)] = MapZone.UNDERSTORE,
        [nameof(PlayerData.SavedFlea_Under_23)] = MapZone.UNDERSTORE,

    };

    public static MapZone GetSanitizedMapZone(MapZone? current = null)
    {
        if (current.HasValue) return current.Value;

        if (GameManager.SilentInstance == null)
        {
            return MapZone.NONE;
        }

        try
        {
            return GameManager.SilentInstance.gameMap.GetMapZoneFromSceneName(SceneManager.GetActiveScene().name);
        }
        catch (Exception)
        {
            return GameManager.SilentInstance.GetCurrentMapZoneEnum();
        }
        
    }

    public override string GetText()
    {
        MapZone mapZone = GetSanitizedMapZone();
        int fleasCollected = FleaBools.Keys.Where(x => PlayerData.instance.GetBool(x)).Count();
        int areaFleas = FleaBools
            .Where(kvp => GetSanitizedMapZone(kvp.Value) == mapZone)
            .Where(kvp => PlayerData.instance.GetBool(kvp.Key))
            .Count();

        return $"{fleasCollected}({areaFleas})";
    }

    public override void SetupHooks()
    {
        PlayerDataVariableEvents<bool>.OnSetVariable += OnSetBool;
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnSceneChange(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene scene)
    {
        UpdateText(showRule: ShowRule.DontShow);
    }

    private bool OnSetBool(PlayerData pd, string fieldName, bool current)
    {
        if (current && FleaBools.ContainsKey(fieldName))
        {
            UpdateTextNextFrame();
        }

        return current;
    }
}
