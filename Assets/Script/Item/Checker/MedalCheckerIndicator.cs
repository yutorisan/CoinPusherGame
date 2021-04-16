using UnityEngine;
using UniRx;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// MedalCheckerにメダルがはいったら発光させる
    /// </summary>
    public class MedalCheckerIndicator : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private Light indicator;
        [SerializeField, Required]
        private IObservableMedalChecker checker;

        private void Start()
        {
            //インジケータが発行してもとに戻るTween
            var tween = indicator.DOIntensity(1.5f, .5f)
                                 .SetLoops(2, LoopType.Yoyo)
                                 .SetAutoKill(false);
            //チェッカーが検知したらTweenを再生
            checker.Checked.Subscribe(_ => tween.Restart());
        }
    }
}