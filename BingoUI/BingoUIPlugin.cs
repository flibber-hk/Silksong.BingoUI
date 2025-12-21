using BepInEx;
using BingoUI.Data;
using MonoDetour.HookGen;
using Newtonsoft.Json;
using Silksong.DataManager;
using Silksong.GameObjectDump.Logging;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BingoUI;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.flibber-hk.bingoui")]
[BepInDependency("org.silksong-modding.datamanager")]
[MonoDetourTargets(typeof(UIManager))]
[MonoDetourTargets(typeof(HeroController))]
public partial class BingoUIPlugin : BaseUnityPlugin, ISaveDataMod<SaveData>
{
    public static BingoUIPlugin Instance { get; private set; }
    SaveData? ISaveDataMod<SaveData>.SaveData
    {
        get => SaveData.Instance;
        set => SaveData.Instance = value;
    }

    internal CounterManager CounterManager;
    private CanvasManager? _canvasManager;

    private void Awake()
    {
        Instance = this;

        ConfigSettings.Setup(Config);

        CurrencyTracker.Hook();

        CounterManager = new();

        Md.HeroController.Start.Postfix(SetupCanvas);
        Md.UIManager.GoToPauseMenu.Postfix(AfterPause);
        Md.UIManager.UIClosePauseMenu.Postfix(AfterUnpause);
        Md.UIManager.ReturnToMainMenu.Postfix(TakedownCanvas);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    private void TakedownCanvas(UIManager self, ref IEnumerator returnValue)
    {
        returnValue = Wrapped(returnValue);

        static IEnumerator Wrapped(IEnumerator orig)
        {
            yield return orig;

            Instance._canvasManager?.Dispose();
            Instance._canvasManager = null;
        }
    }

    private void AfterUnpause(UIManager self)
    {
        if (ConfigSettings.AlwaysDisplayCounters) return;

        if (_canvasManager != null)
        {
            _canvasManager.FadeOutAll();
        }
    }

    private void AfterPause(UIManager self, ref IEnumerator returnValue)
    {
        returnValue = Wrapped(returnValue);

        static IEnumerator Wrapped(IEnumerator orig)
        {
            yield return orig;

            if (ConfigSettings.AlwaysDisplayCounters || ConfigSettings.NeverDisplayCounters)
            {
                yield break;
            }

            if (Instance._canvasManager != null)
            {
                Instance._canvasManager.FadeInAll();
            }
        }
    }

    private void SetupCanvas(HeroController self)
    {
        _canvasManager = new();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (_canvasManager is null) return;

        if (!ConfigSettings.AlwaysDisplayCounters)
        {
            _canvasManager.FadeOutAll();
        }
    }
}
