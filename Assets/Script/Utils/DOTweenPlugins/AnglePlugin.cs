using System;
using System.ComponentModel;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityUtility;

public class AnglePlugin : ABSTweenPlugin<Angle, Angle, AngleOptions>
{
    private static AnglePlugin _instance;
    public static AnglePlugin Instance => _instance ?? (_instance = new AnglePlugin());

    private AnglePlugin() { }

    public override Angle ConvertToStartValue(TweenerCore<Angle, Angle, AngleOptions> t, Angle value)
    {
        //T1とT2は同じなのでただreturnするだけ
        return value;
    }


    public override void SetChangeValue(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        //TweenerCoreのchangeValueに開始値と終了値の差分を計算して代入する
        var diffAngle = t.endValue - t.startValue;
        //正転オプションが有効になっていたら、AngleをPositiveNormalizeして正転方向に正規化する
        switch (t.plugOptions.Direction)
        {
            case AngleTweenDirection.Forward:
                if (diffAngle.IsPositive) t.changeValue = diffAngle;
                else t.changeValue = diffAngle.Reverse();
                break;
            case AngleTweenDirection.Backward:
                if (!diffAngle.IsPositive) t.changeValue = diffAngle;
                else t.changeValue = diffAngle.Reverse();
                break;
            default:
                t.changeValue = diffAngle;
                break;
        }
    }

    public override void SetRelativeEndValue(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        t.endValue += t.startValue;
    }


    public override void SetFrom(TweenerCore<Angle, Angle, AngleOptions> t, bool isRelative)
    {
        //開始値と終了値を反転させる
        Angle prevEndAngle = t.endValue;
        t.endValue = t.getter();
        t.startValue = isRelative ? t.endValue + prevEndAngle : prevEndAngle;
        t.setter(t.startValue);
    }

    public override void SetFrom(TweenerCore<Angle, Angle, AngleOptions> t, Angle fromValue, bool setImmediately, bool isRelative)
    {
        //引数で指定された値を開始値に設定する
        t.startValue = fromValue;
        if (setImmediately) t.setter(fromValue);
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
        //イージング係数を取得
        float ease = EaseManager.Evaluate(t, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
        //setterに更新された値を渡す
        setter(startValue + changeValue * ease);
    }

    public override float GetSpeedBasedDuration(AngleOptions options, float unitsXSecond, Angle changeValue)
    {
        switch (options.AngularVelocityUnit)
        {
            case AngularVelocityUnit.DegreePerSecond:
                return changeValue.TotalDegree / unitsXSecond;
            case AngularVelocityUnit.RadianPerSecond:
                return changeValue.TotalRadian / unitsXSecond;
            case AngularVelocityUnit.Unspecified:
                throw new InvalidOperationException("角速度の単位が指定されていません。");
            default:
                throw new InvalidEnumArgumentException();
        }

    }

    public override void Reset(TweenerCore<Angle, Angle, AngleOptions> t)
    {
        //何もしない
    }
}
