using BepInEx.Configuration;
using BingoUI.Data;
using Silksong.UnityHelper.Extensions;
using System;

namespace BingoUI;

public abstract class AbstractCounter(string spriteName)
{
    internal static event Action<string, ShowRule>? OnUpdateText;

    /// <summary>
    /// The name of the sprite, which also serves as the key for this counter
    /// and so should be unique.
    /// </summary>
    public string SpriteName { get; private set; } = spriteName;

    /// <summary>
    /// Function used to determine the text of the counter.
    /// </summary>
    public abstract string GetText();

    /// <summary>
    /// Function used to setup callbacks.
    /// 
    /// This should always be used to decide when to show the counter, typically by calling
    /// <see cref="UpdateTextNextFrame"/>, but may also be used to setup callbacks
    /// for tracking saved data.
    /// </summary>
    public abstract void SetupHooks();

    /// <summary>
    /// Set the text displayed by the counter. By default, show the counter either if it's already on screen or
    /// the text changed.
    /// </summary>
    /// <param name="showRule">Rule about whether or not to show the counter.</param>
    protected internal void UpdateText(ShowRule showRule = ShowRule.Default)
    {
        OnUpdateText?.Invoke(SpriteName, showRule);
    }

    /// <summary>
    /// Wait a frame, and then call <see cref="UpdateText"/>.
    /// 
    /// This is intended to be used in callbacks that run before save data modifications happen.
    /// </summary>
    /// <inheritdoc cref="UpdateText"/>
    protected void UpdateTextNextFrame(ShowRule showRule = ShowRule.Default)
    {
        BingoUIPlugin.Instance.InvokeNextFrame(() => UpdateText(showRule));
    }

    /// <summary>
    /// Whether or not this counter is allowed to be shown.
    /// This is only checked when building the canvas.
    /// </summary>
    protected virtual bool CounterDisplayEnabled => true;

    internal bool IsCounterDisplayEnabled
    {
        get
        {
            if (!CounterDisplayEnabled) return false;
            if (ConfigSettings.CounterSettings?.TryGetValue(SpriteName, out ConfigEntry<bool> entry) ?? false)
            {
                return entry.Value;
            }

            return true;
        }
    }
}