using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出を決定できる
    /// </summary>
    public interface ISlotProductionDeterminer
    {
        void DetermineProduction(RoleSet roleSet);
    }
    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    public class SlotProductionDeterminer : SerializedMonoBehaviour, ISlotProductionDeterminer
    {
        [Inject]
        private ISlotDriver m_slotDriver;

        public void DetermineProduction(RoleSet roleSet)
        {
            m_slotDriver.ControlBy(new Production(roleSet));
        }
    }
}