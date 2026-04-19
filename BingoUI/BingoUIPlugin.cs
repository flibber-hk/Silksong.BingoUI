using BepInEx;
using BingoUI.Data;
using MonoDetour.Reflection.Unspeakable;
using Silksong.DataManager;
using Silksong.ModMenu.Plugin;
using Silksong.ModMenu.Screens;
using Silksong.UnityHelper.Extensions;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BingoUI;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.flibber-hk.bingoui")]
[BepInDependency("org.silksong-modding.datamanager")]
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

        CounterManager = new();
        ConfigSettings.Setup(Config, CounterManager);

        CurrencyTracker.Hook();

        Md.HeroController.Start.Postfix(SetupCanvas);
        Md.UIManager.GoToPauseMenu.Postfix(AfterPause);
        Md.UIManager.UIClosePauseMenu.Postfix(AfterUnpause);
        Md.UIManager.ReturnToMainMenu.Postfix(TakedownCanvas);
        Md.UIManager.ShowMenu.PostfixMoveNext(HideCountersInModMenu);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Config.SettingChanged += OnConfigSettingChanged;

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    private void HideCountersInModMenu(SpeakableEnumerator<object, UIManager> self, ref bool continueEnumeration)
    {
        // When the ShowMenu coroutine has finished, check if the new menu screen is modded and 
        if (continueEnumeration) return;

        if (ConfigSettings.HideInModMenu?.Value == false)
        {
            return;
        }

        // Wait to fade out because the history is changed only after the coroutine is finished
        this.InvokeAfterFrames(() =>
        {
            if (MenuScreenNavigation.CurrentModMenuScreen == null)
            {
                // Not a mod menu
                return;
            }

            if (_canvasManager != null)
            {
                _canvasManager.FadeOutAll();
            }
        }, 2);
    }

    private void OnConfigSettingChanged(object sender, BepInEx.Configuration.SettingChangedEventArgs e)
    {
        if (e.ChangedSetting.Definition.Section.StartsWith("Counters") &&
            (UIManager.instance.uiState == GlobalEnums.UIState.PAUSED || UIManager.instance.uiState == GlobalEnums.UIState.PLAYING)
            )
        {
            SetupCanvas();
        }
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

    private void SetupCanvas(HeroController self) => SetupCanvas();
    
    private void SetupCanvas()
    {
        _canvasManager?.Dispose();
        _canvasManager = new();

        if (!ConfigSettings.NeverDisplayCounters && UIManager.instance.uiState == GlobalEnums.UIState.PAUSED)
        {
            _canvasManager.FadeInAll();
        }
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
