using System;
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

[Flags]
public enum RelicTypes
{
    Unknown = 0,

    ArcaneEgg = 1,
    BoneScroll = 2,
    ChoralCommandment = 4,
    RuneHarp = 8,
    WeaverEffigy = 16,
    PsalmCylinder = 32,
    SacredCylinder = 64,

    AnyRelic = ArcaneEgg | BoneScroll | ChoralCommandment | RuneHarp | WeaverEffigy,
    AnyCylinder = PsalmCylinder | SacredCylinder,
    Any = AnyRelic | AnyCylinder,
}