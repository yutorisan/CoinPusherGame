using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityUtility;

public class AnglePlugin : ABSTweenPlugin<Angle, Angle, NoOptions>
{
    private static AnglePlugin _instance;
    public static AnglePlugin Instance => _instance ?? (_instance = new AnglePlugin());

    private AnglePlugin() { }

    public override Angle ConvertToStartValue(TweenerCore<Angle, Angle, NoOptions> t, Angle value)
    {
        return value;
    }

    public override void EvaluateAndApply(NoOptions options,
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

        startValue += changeValue * ease;
        setter(startValue);
    }

    public override float GetSpeedBasedDuration(NoOptions options, float unitsXSecond, Angle changeValue)
    {
        throw new NotImplementedException();
    }

    public override void Reset(TweenerCore<Angle, Angle, NoOptions> t)
    {
        //何もしない
    }

    public override void SetChangeValue(TweenerCore<Angle, Angle, NoOptions> t)
    {
        t.changeValue = t.endValue - t.startValue;
    }

    public override void SetFrom(TweenerCore<Angle, Angle, NoOptions> t, bool isRelative)
    {
        throw new NotImplementedException();
    }

    public override void SetFrom(TweenerCore<Angle, Angle, NoOptions> t, Angle fromValue, bool setImmediately, bool isRelative)
    {
        throw new NotImplementedException();
    }

    public override void SetRelativeEndValue(TweenerCore<Angle, Angle, NoOptions> t)
    {
        throw new NotImplementedException();
    }
}
