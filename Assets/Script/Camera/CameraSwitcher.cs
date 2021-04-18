using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MedalPusher.Lottery;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Camera
{
    /// <summary>
    /// ゲーム状況に応じてカメラ位置を変更する
    /// </summary>
    public class CameraSwitcher : MonoBehaviour
    {
        [Inject]
        private IObservableLotteryStatus lotteryStatus;

        [SerializeField]
        private Transform mainTransform;
        [SerializeField]
        private Transform lotteryTransform;

        void Start()
        {
            //Lotterが抽選中ならカメラをLotteryに、
            //そうでなかったらデフォルトの位置にする
            lotteryStatus.Status
                .DistinctUntilChanged()
                .Select(status => status == LotteryStatus.Lotterying ? lotteryTransform : mainTransform)
                .Subscribe(targetTr =>
                {
                    transform.DOMove(targetTr.position, 0.5f).Play();
                    transform.DORotateQuaternion(targetTr.rotation, 0.5f).Play();
                });
        }
    }
}
