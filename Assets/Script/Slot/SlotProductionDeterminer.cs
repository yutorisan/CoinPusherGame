using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        private NormalRollProductionProperty m_normalProp;
        [SerializeField]
        private ReachProductionProperty m_reachProp;

        public UniTask DetermineProduction(Scenario scenario)
        {
            ReachProductionProperty reachProp = m_reachProp;
            reachProp.AntagonismType = ReachAntagonismProduction.Antagonism;

            Production production = new Production(scenario, m_normalProp, reachProp);

            return m_slotDriver.ControlBy(production);
        }
    }
}