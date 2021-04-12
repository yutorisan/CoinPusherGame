using System;
using DG.Tweening;

namespace MedalPusher.Utils
{
    public static class DOTweenEx
    {
        /// <summary>
        /// TweenをSequenceに変換する
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static Sequence ToSequence(this Tween tween) =>
            DOTween.Sequence().Append(tween);
    }
}