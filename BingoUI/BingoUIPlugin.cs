using BepInEx;
using MonoDetour.HookGen;
using MonoDetour.Reflection.Unspeakable;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BingoUI;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.flibber-hk.bingoui")]
[MonoDetourTargets(typeof(UIManager))]
[MonoDetourTargets(typeof(HeroController))]
public partial class BingoUIPlugin : BaseUnityPlugin
{
    public static BingoUIPlugin Instance { get; private set; }

    internal CounterManager CounterManager;
    private CanvasManager? _canvasManager;

    private void Awake()
    {
        Instance = this;

        // Can't add this for now because getting the text looking nice is nontrivial...
        // Also means I don't have to deal with saving data yet :zote:
        // CurrencyTracker.Hook();

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
        if (GlobalSettingsProxy.AlwaysDisplay) return;

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

            if (GlobalSettingsProxy.AlwaysDisplay || GlobalSettingsProxy.NeverDisplay)
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

        if (!GlobalSettingsProxy.AlwaysDisplay)
        {
            _canvasManager.FadeOutAll();
        }
    }
}
