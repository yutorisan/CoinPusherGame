using System;
using System.Runtime.CompilerServices;
using MedalPusher.Item.Pool;
using UniRx;
using UnityEngine;
using UnityUtility;
using Zenject;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// メダルを払い出す
    /// </summary>
    public class NormalMedalPayouter : MedalPayouter
    {
        /// <summary>
        /// 払出しポイント
        /// </summary>
        [SerializeField]
        private Transform m_payoutPoint;
        /// <summary>
        /// 払出しの間隔
        /// </summary>
        private TimeSpan m_payoutInterval = TimeSpan.FromMilliseconds(100);

        private void Start()
        {
            PowerOnTiming.SelectMany(_ => Observable.Interval(m_payoutInterval)
                                                        //ストックが0になるまで
                                                    .TakeUntil(PayoutStock.Where(n => n == 0))
                                                        //0になったらステータスをIdolにする
                                                    .DoOnCompleted(() => m_status = PayoutStatus.Idol))
                         //メダルを投入
                         .Subscribe(_ => PayoutToField(m_payoutPoint.position));
        }
    }
}