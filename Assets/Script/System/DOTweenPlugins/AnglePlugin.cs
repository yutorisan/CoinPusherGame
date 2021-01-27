using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityUtility;

public class AnglePlugin : ABSTweenPlugin<Angle, Angle, AngleOptions>
{
    private static AnglePlugin _instance;
    public static AnglePlugin Instance => _instance ?? (_instance = new AnglePlugin());

    private AnglePlugin() { }

    public override Angle ConvertToStartValue(TweenerCore<Angle, Angle, AngleOptions> t, Angle value)
    {
        return value;
    }

    public override void EvaluateAndApply(AngleOptions options,
                                          Tween t,
                                          bool isRelative,
                                          DOGetter<Angle> getter,
                                          DOSetter<Angle> setter,
                                          float elapsed,
                                          Angle startValue,
                                          Angle changeValue,
                                          float duration,
                                          bool usingInversePosition,
                                          UpdateNotice updateNotice)
    {
        float ease = EaseManager.Evaluate(t, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
        setter(startValue + changeValue * ease);
    }

    public override float GetSpeedBasedDuration(AngleOptions options, float unitsXSecond, Angle changeValue)
    {
        throw new NotImplementedException();
    }

    public override void Reset(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        //何もしない
    }

    public override void SetChangeValue(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        var diffAngle = t.endValue - t.startValue;
        if (t.plugOptions.Direction == AngleTweenDirection.Forward) t.changeValue = diffAngle.PositiveNormalize();
        else t.changeValue = diffAngle;
    }

    public override void SetFrom(TweenerCore<Angle, Angle, AngleOptions> t, bool isRelative)
    {
        throw new NotImplementedException();
    }

    public override void SetFrom(TweenerCore<Angle, Angle, AngleOptions> t, Angle fromValue, bool setImmediately, bool isRelative)
    {
        throw new NotImplementedException();
    }

    public override void SetRelativeEndValue(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        throw new NotImplementedException();
    }
}
