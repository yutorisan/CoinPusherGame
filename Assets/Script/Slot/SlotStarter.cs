using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Stock;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの抽選を開始させる
    /// </summary>
    public class SlotStarter : MonoBehaviour
    {
        [Inject]
        private ISlotRoleDeterminer m_roleDeterminer;
        [Inject]
        private IStockCounter m_stockCounter;
        /// <summary>
        /// スロットを回転させていいかどうか
        /// </summary>
        private bool m_isAllowedStart = true;
        /// <summary>
        /// スロット回転完了通知を受け取ったらそれをクラス内に伝えるためのSubject
        /// </summary>
        private readonly Subject<Unit> m_slotOnCompletedSubject = new Subject<Unit>();

        // Start is called before the first frame update
        void Start()
        {
            //スロットの抽選が完了したら、スロット回転可能にする
            m_slotOnCompletedSubject.Subscribe(_ => m_isAllowedStart = true);

            //スロットの開始条件が整ったらスロット開始を依頼する
            m_stockCounter.StockSupplied            //ストックが供給された、かつ
                          .Where(_ => m_isAllowedStart) //スロットを回転させても良い
                          //または、前のスロットの回転が終了して、かつまだストックがある
                          .Merge(m_slotOnCompletedSubject.Where(_ => m_stockCounter.IsSpendable))                           .SelectMany(_ =>
                          { //ならば、ストックを消費してスロットの回転を開始させる
                              m_stockCounter.SpendStock();
                              m_isAllowedStart = false;
                              return m_roleDeterminer.DetermineRole();
                          }) //スロットの回転完了通知をもらったら、それをクラス内に伝達させる
                          .Subscribe(_ => m_slotOnCompletedSubject.OnNext(Unit.Default));

        }
    }
}