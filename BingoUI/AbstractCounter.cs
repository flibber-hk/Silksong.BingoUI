using System;

namespace BingoUI;

public abstract partial class AbstractCounter
{
    internal static event Action<string, ShowRule>? OnUpdateText;

    public float x { get; set; }
    public float y { get; set; }

    /// <summary>
    /// The name of the sprite, which also serves as the key for this counter
    /// and so should be unique.
    /// </summary>
    public string SpriteName { get; set; }

    /// <summary>
    /// Function used to determine the text of the counter.
    /// </summary>
    public abstract string GetText();

    /// <summary>
    /// Function used to setup callbacks.
    /// </summary>
    public abstract void Hook();

    /// <summary>
    /// Set the text displayed by the counter. By default, show the counter either if it's already on screen or
    /// the text changed.
    /// </summary>
    /// <param name="showRule">Rule about whether or not to show the counter.</param>
    protected internal void UpdateText(ShowRule showRule = ShowRule.Default)
    {
        OnUpdateText?.Invoke(SpriteName, showRule);
    }

    public AbstractCounter(float x, float y, string spriteName)
    {
        this.SpriteName = spriteName;
        this.x = x;
        this.y = y;
    }
}