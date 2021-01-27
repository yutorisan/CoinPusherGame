using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Stock;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using Sirenix.OdinInspector;
using MedalPusher.Slot.Prize;

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
            //スロットの開始条件が整ったらスロット開始を依頼する
            m_stockCounter.StockSupplied            //ストックが供給された、かつ
                          .Where(_ => m_isAllowedStart) //スロットを回転させても良い
                                                        //または、前のスロットの回転が終了して、かつまだストックがある
                          .Merge(m_slotOnCompletedSubject.Where(_ => m_stockCounter.IsSpendable))
                          .Subscribe(async _ =>
                          { //ならば、ストックを消費してスロットの回転を開始させる
                              m_stockCounter.SpendStock();
                              m_isAllowedStart = false;
                              await m_roleDeterminer.DetermineRole();
                              m_isAllowedStart = true;
                              m_slotOnCompletedSubject.OnNext(Unit.Default);
                          });

        }

        
    }
}