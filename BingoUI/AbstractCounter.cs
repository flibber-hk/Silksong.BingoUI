using Silksong.UnityHelper.Extensions;
using System;

namespace BingoUI;

public abstract partial class AbstractCounter(string spriteName)
{
    internal static event Action<string, ShowRule>? OnUpdateText;

    /// <summary>
    /// The name of the sprite, which also serves as the key for this counter
    /// and so should be unique.
    /// </summary>
    public string SpriteName { get; set; } = spriteName;

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

    protected void UpdateTextNextFrame(ShowRule showRule = ShowRule.Default)
    {
        BingoUIPlugin.Instance.InvokeNextFrame(() => UpdateText(showRule));
    }
}