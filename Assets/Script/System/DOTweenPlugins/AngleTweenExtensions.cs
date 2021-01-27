using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityUtility;

public static class AngleTweenExtensions
{
    public static Tweener SetOptions(this TweenerCore<Angle, Angle, AngleOptions> t, AngleTweenDirection direction)
    {
        if (t == null || !t.active) return t;
        t.plugOptions.Direction = direction;
        return t;
    }
}
