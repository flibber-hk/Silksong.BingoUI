using Silksong.UnityHelper.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CUtil = CanvasUtil.CanvasUtil;
using UObject = UnityEngine.Object;

namespace BingoUI;

internal class CanvasManager : IDisposable
{
    private GameObject _canvas = null!;

    private Dictionary<string, (CanvasGroup canvasGroup, Text text)> _canvasPanels = new();
    private Dictionary<string, DateTime> _nextCanvasFade = new();

    public CanvasManager()
    {
        _canvas = CUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));
        UObject.DontDestroyOnLoad(_canvas);
        AbstractCounter.OnUpdateText += OnUpdateText;
    
        foreach (AbstractCounter counter in BingoUIPlugin.Instance.CounterManager.Counters)
        {
            SetupCanvasIcon(counter);
        }
    }

    public void Dispose()
    {
        AbstractCounter.OnUpdateText -= OnUpdateText;
        UObject.Destroy(_canvas);
    }

    public void SetupCanvasIcon(AbstractCounter counter)
    {
        Vector2 anchor = new(counter.x, counter.y);
        GameObject canvasSprite = CUtil.CreateImagePanel
        (
            _canvas,
            SpriteUtil.LoadEmbeddedSprite(GetType().Assembly, $"BingoUI.Resources.Images.{counter.SpriteName}.png"),
            new CanvasUtil.RectData(Vector2.zero, Vector2.zero, anchor, anchor + new Vector2(1f / 15f, 0.1f))
        );

        // Add a canvas group so we can fade it in and out
        CanvasGroup canvasGroup = canvasSprite.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        if (!GlobalSettingsProxy.AlwaysDisplay) canvasGroup.gameObject.SetActive(false);

        GameObject textPanel = CUtil.CreateTextPanel
        (
            canvasSprite,
            "0",
            23,
            TextAnchor.LowerCenter,
            new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
        );
        Text text = textPanel.GetComponent<Text>();
        text.color = Color.black;

        _canvasPanels[counter.SpriteName] = (canvasGroup, text);
    }

    public void OnUpdateText(string key, ShowRule showRule)
    {
        AbstractCounter counter = BingoUIPlugin.Instance.CounterManager.CounterLookup[key];
        (CanvasGroup canvasGroup, Text text) = _canvasPanels[counter.SpriteName];

        string newText = counter.GetText();

        if (text.text == newText && showRule != ShowRule.ForceShow) return;
        text.text = newText;
        if (showRule == ShowRule.DontShow) return;

        if (DateTime.Now >= (_nextCanvasFade.TryGetValue(counter.SpriteName, out DateTime dt) ? dt : DateTime.MinValue))
        {
            BingoUIPlugin.Instance.StartCoroutine(FadeCanvas(canvasGroup));
            _nextCanvasFade[counter.SpriteName] = DateTime.Now.AddSeconds(0.5f);
        }

    }

    public void FadeInAll()
    {
        foreach ((string key, (CanvasGroup cg, Text text)) in _canvasPanels)
        {
            text.text = BingoUIPlugin.Instance.CounterManager.CounterLookup[key].GetText();

            FadeIn(cg);
        }
    }

    public void FadeOutAll()
    {
        foreach ((CanvasGroup cg, Text text) in _canvasPanels.Values)
        {
            FadeOut(cg);
        }
    }

    private IEnumerator FadeCanvas(CanvasGroup cg)
    {
        if (GlobalSettingsProxy.AlwaysDisplay || GlobalSettingsProxy.NeverDisplay) yield break;

        if (!cg.gameObject.activeSelf) FadeIn(cg);
        yield return new WaitForSeconds(4f);
        FadeOut(cg);
    }

    public void FadeIn(CanvasGroup cg)
    {
        BingoUIPlugin.Instance.StartCoroutine(CUtil.FadeInCanvasGroup(cg));
    }

    public void FadeOut(CanvasGroup cg)
    {
        BingoUIPlugin.Instance.StartCoroutine(CUtil.FadeOutCanvasGroup(cg));
    }

}
