using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <param name="roleSet">演出の結果、最終的に表示する出目</param>
        /// <returns>スロットの演出を決定して回したスロットの回転が完了したことを知らせる通知</returns>
        IObservable<Unit> DetermineProduction(RoleSet roleSet);
    }
    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    public class SlotProductionDeterminer : SerializedMonoBehaviour, ISlotProductionDeterminer
    {
        [Inject]
        private ISlotDriver m_slotDriver;

        public IObservable<Unit> DetermineProduction(RoleSet roleSet)
        {
            return m_slotDriver.ControlBy(new Production(roleSet));
        }
    }
}