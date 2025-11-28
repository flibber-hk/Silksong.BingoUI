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
