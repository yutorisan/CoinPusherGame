using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityUtility;

public static class AngleTweenExtensions
{
    public static Tweener SetOptions(this TweenerCore<Angle, Angle, AngleOptions> t, AngleTweenDirection direction, AngularVelocityUnit angularVelocityUnit = AngularVelocityUnit.Unspecified)
    {
        if (t == null || !t.active) return t;
        t.plugOptions.Direction = direction;
        t.plugOptions.AngularVelocityUnit = angularVelocityUnit;
        return t;
    }

    public static Tweener SetSpeedBased(this TweenerCore<Angle, Angle, AngleOptions> t, AngularVelocityUnit unit)
    {
        if (t == null || !t.active) return t;
        t.SetSpeedBased();
        t.plugOptions.AngularVelocityUnit = unit;
        return t;
    }
}
