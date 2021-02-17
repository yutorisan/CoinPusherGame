using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
using MedalPusher.Slot.Stock;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出を決定できる
    /// </summary>
    public interface ISlotProductionDeterminer
    {
        /// <summary>
        /// スロットの演出の決定を依頼する
        /// </summary>
        /// <param name="scenario">スロットのシナリオ</param>
        /// <returns>スロットの演出を決定して回したスロットの回転が完了したことを知らせる通知</returns>
        UniTask DetermineProduction(Scenario scenario);
    }
    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    public class SlotProductionDeterminer : SerializedMonoBehaviour, ISlotProductionDeterminer
    {
        [Inject]
        private ISlotDriver m_slotDriver;
        [Inject]
        private IObservableStockCount m_stockCount;

        [SerializeField, LabelText("通常モード"), TitleGroup("通常回転シーケンス")]
        private NormalRollProductionProperty m_normalProp;
        [SerializeField, LabelText("高速モード"), TitleGroup("通常回転シーケンス")]
        private NormalRollProductionProperty m_normalPropFast;
        [SerializeField, LabelText("高速モードストック残数閾値"), TitleGroup("通常回転シーケンス")]
        private int m_fastModeThreshold;
        [SerializeField, TitleGroup("リーチシーケンス")]
        private ReachProductionProperty m_reachProp;

        public UniTask DetermineProduction(Scenario scenario)
        {
            Production production;
            if (m_stockCount.StockCount.Value < m_fastModeThreshold)
            {
                production = new Production(scenario, m_normalProp, m_reachProp);
            }
            else
            {
                production = new Production(scenario, m_normalPropFast, m_reachProp);
            }

            return m_slotDriver.ControlBy(production);
        }
    }
}