using System;
using System.Runtime.CompilerServices;
using MedalPusher.Item.Pool;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using UnityUtility;
using Zenject;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// 通常のメダル払い出しを行う
    /// </summary>
    public class NormalMedalPayouter : MedalPayouter
    {
        /// <summary>
        /// 払出しポイント
        /// </summary>
        [SerializeField, Required]
        private GameObject payoutPoint;
        /// <summary>
        /// 払出しの間隔
        /// </summary>
        private TimeSpan payoutInterval = TimeSpan.FromMilliseconds(100);

        private void Start()
        {
            //メダル払い出し開始要求を受けたら、ストックが0になるまで一定間隔で払い出しを続ける
            PowerOnTiming.SelectMany(_ => Observable.Interval(payoutInterval)
                                                    //ストックが0になるまで
                                                    .TakeUntil(PayoutStock.Where(n => n == 0))
                                                    //0になったらステータスをIdolにする
                                                    .DoOnCompleted(() => status = PayoutStatus.Idol))
                         //メダルを投入
                         .Subscribe(_ => PayoutToField(payoutPoint.transform.position, Quaternion.Euler(90, 0, 0)));
        }
    }
}