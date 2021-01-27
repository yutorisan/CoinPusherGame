using System;
using DG.Tweening.Plugins.Options;

public struct AngleOptions : IPlugOptions
{
    public void Reset()
    {
        Direction = AngleTweenDirection.Both;
    }

    public AngleTweenDirection Direction { get; set; }
}

public enum AngleTweenDirection
{
    Both,
    Forward,
    Backward
}