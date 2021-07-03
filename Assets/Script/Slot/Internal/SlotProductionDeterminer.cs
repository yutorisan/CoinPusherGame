using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
using MedalPusher.Slot.Internal.Stock;
using MedalPusher.Slot;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;


namespace MedalPusher.Slot.Internal
{

    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    internal class SlotProductionDeterminer : SerializedMonoBehaviour, ISlotProductionDeterminer
    {
        [Inject]
        private ISlotDriver slotDriver;
        [Inject]
        private IObservableStockCount stockCount;

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
            Slot.Production production;
            //ストックの残数を確認して、閾値より多かったら高速回転モードにする
            if (stockCount.StockCount.Value < m_fastModeThreshold)
            {
                production = new Slot.Production(scenario, m_normalProp, m_reachProp);
            }
            else
            {
                production = new Slot.Production(scenario, m_normalPropFast, m_reachProp);
            }

            //決定した演出に従ってスロットを制御する
            return slotDriver.DriveBy(production);
        }
    }

    /// <summary>
    /// スロットの動きを制御する
    /// </summary>
    internal interface ISlotDriver
    {
        /// <summary>
        /// 指定した演出に従ってスロットを制御する
        /// </summary>
        /// <param name="production"></param>
        /// <returns>スロットの制御タスク</returns>
        UniTask DriveBy(Slot.Production production);
    }
}