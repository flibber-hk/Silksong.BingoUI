using System.ComponentModel;

namespace BingoUI;

public enum ShowRule
{
    Default,

    /// <summary>
    /// Show the tracker even if the text hasn't updated.
    /// </summary>
    ForceShow,

    /// <summary>
    /// Don't show the tracker even if the text has updated.
    /// </summary>
    DontShow,
}

public enum DisplayMode
{
    [Description("Show and hide counters")]
    Default,

    [Description("Show counters when possible")]
    AlwaysDisplay,

    [Description("Never show counters")]
    NeverDisplay
}