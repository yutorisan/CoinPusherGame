using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MedalPusher.Item.Checker;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace MedalPusher.Effects
{
    /// <summary>
    /// メダル獲得時に、現在の所持メダル数Viewがポップアップするエフェクト
    /// </summary>
    public class MedalInventoryViewPopupEffect : SerializedMonoBehaviour
    {
        /// <summary>
        /// エフェクト再生開始トリガーとなるMedalChecker
        /// </summary>
        [SerializeField]
        private IObservableMedalChecker medalChecker;
        /// <summary>
        /// エフェクトの継続時間
        /// </summary>
        [SerializeField]
        private float duration;

        void Start()
        {
            //再利用できるエフェクト本体を定義する
            RectTransform rectTransform = transform as RectTransform;
            Tweener tweener =
                rectTransform.DOPunchScale(new Vector3(.1f, .1f, 0f), duration, 2)
                             .SetEase(Ease.OutQuint)
                             .SetRelative()
                             .SetRecyclable()
                             .SetAutoKill(false);

            //トリガーがかかったらポップアップエフェクトを開始させる
            medalChecker.Checked.Subscribe(_ =>
            {
                //エフェクト完了なら初期化する
                if (tweener.IsComplete()) tweener.Rewind();
                //前のエフェクトの途中なら最初から再生し直す
                if (tweener.IsPlaying()) tweener.Restart();
                //そうでないなら最初から再生する
                else tweener.Play();
            });
        }
    }
}