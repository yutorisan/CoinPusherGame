using System;
using DG.Tweening;

public static class DOTweenEx
{
    public static Sequence ToSequence(this Tween tween) =>
        DOTween.Sequence().Append(tween);
}
