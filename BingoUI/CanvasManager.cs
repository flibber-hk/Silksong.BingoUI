using BingoUI.Data;
using Silksong.UnityHelper.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CUtil = CanvasUtil.CanvasUtil;
using UObject = UnityEngine.Object;

namespace BingoUI;

internal class CanvasManager : IDisposable
{
    private class CounterData
    {
        public required CanvasGroup CanvasGroup { get; set; }
        public required Text Text { get; set; }
        public DateTime NextCanvasFade { get; set; } = DateTime.MinValue;
    }

    private GameObject _canvas;

    private Dictionary<string, CounterData> _canvasPanels;

    public CanvasManager()
    {
        _canvas = CUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));
        UObject.DontDestroyOnLoad(_canvas);
        AbstractCounter.OnUpdateText += OnUpdateText;

        _canvasPanels = [];

        using (IEnumerator<Vector2> positionEnumerator = GetDefaultPositions().GetEnumerator())
        {
            foreach (AbstractCounter counter in BingoUIPlugin.Instance.CounterManager.Counters.Values)
            {
                if (!counter.IsCounterDisplayEnabled) continue;

                positionEnumerator.MoveNext();
                Vector2 position = positionEnumerator.Current;
                CounterData cd = SetupCanvasIcon(counter, position);
                _canvasPanels[counter.SpriteName] = cd;
            }
        }
    }

    private static IEnumerable<Vector2> GetDefaultPositions()
    {
        float y = 0.01f;
        while (y < 0.9f)
        {
            for (int x = 14; x >= 0; x--)
            {
                yield return new(x / 15f, y);
            }
            y += 0.11f;
        }
    }

    public void Dispose()
    {
        AbstractCounter.OnUpdateText -= OnUpdateText;
        UObject.Destroy(_canvas);
    }

    private CounterData SetupCanvasIcon(AbstractCounter counter, Vector2 position)
    {
        Vector2 anchor = position;
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
        if (!ConfigSettings.AlwaysDisplayCounters) canvasGroup.gameObject.SetActive(false);

        GameObject textPanel = CUtil.CreateTextPanel
        (
            canvasSprite,
            string.Empty,
            23,
            TextAnchor.LowerCenter,
            new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
        );
        Text text = textPanel.GetComponent<Text>();
        text.color = Color.black;

        return new() { CanvasGroup = canvasGroup, Text = text };
    }

    public void OnUpdateText(string key, ShowRule showRule)
    {
        if (!BingoUIPlugin.Instance.CounterManager.Counters.TryGetValue(key, out AbstractCounter counter))
        {
            // This shouldn't happen
            return;
        }

        if (!_canvasPanels.TryGetValue(counter.SpriteName, out CounterData data))
        {
            // If a counter is active but doesn't have a canvas panel, return silently
            return; 
        }

        string newText = counter.GetText();

        if (data.Text.text == newText && showRule != ShowRule.ForceShow) return;
        data.Text.text = newText;
        if (showRule == ShowRule.DontShow) return;

        if (DateTime.Now >= data.NextCanvasFade)
        {
            BingoUIPlugin.Instance.StartCoroutine(FadeCanvas(data.CanvasGroup));
            data.NextCanvasFade = DateTime.Now.AddSeconds(0.5f);
        }

    }

    public void FadeInAll()
    {
        foreach ((string key, CounterData cd) in _canvasPanels)
        {
            cd.Text.text = BingoUIPlugin.Instance.CounterManager.Counters[key].GetText();

            FadeIn(cd.CanvasGroup);
        }
    }

    public void FadeOutAll()
    {
        foreach (CounterData cd in _canvasPanels.Values)
        {
            FadeOut(cd.CanvasGroup);
        }
    }

    private static IEnumerator FadeCanvas(CanvasGroup cg)
    {
        if (ConfigSettings.AlwaysDisplayCounters || ConfigSettings.NeverDisplayCounters) yield break;

        if (!cg.gameObject.activeSelf) FadeIn(cg);
        yield return new WaitForSeconds(4f);
        FadeOut(cg);
    }

    private static void FadeIn(CanvasGroup cg)
    {
        BingoUIPlugin.Instance.StartCoroutine(CUtil.FadeInCanvasGroup(cg));
    }

    private static void FadeOut(CanvasGroup cg)
    {
        BingoUIPlugin.Instance.StartCoroutine(CUtil.FadeOutCanvasGroup(cg));
    }

}
