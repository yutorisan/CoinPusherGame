using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台上に存在するボール数を購読できる
    /// </summary>
    internal interface IObservableBallCount
    {
        /// <summary>
        /// 抽選台上に存在するボール数
        /// </summary>
        IReadOnlyReactiveProperty<int> BallCount { get; }
    }
    /// <summary>
    /// 抽選台上に存在するボール数を管理する
    /// </summary>
    internal class OnLotteryBallCounter : IObservableBallCount, IInitializable
    {
        [Inject]
        private IObservableBallBorned ballBorned;

        private readonly ReactiveProperty<int> onCount = new ReactiveProperty<int>();

        public IReadOnlyReactiveProperty<int> BallCount => onCount;

        public void Initialize()
        {
            //ボールが生成された通知を受けたらカウントをインクリメントする
            ballBorned.BallBorned.Subscribe(_ => ++onCount.Value);

        }
    }
}