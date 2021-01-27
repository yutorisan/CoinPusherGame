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

        public UniTask DetermineProduction(Scenario scenario)
        {
            Production production = new Production(scenario);

            //if (scenario.FirstRoleset.IsReach)
            //{ //リーチである
            //    //リーチ後に当たりにするかどうかによって、リーチのリールの最終的な出目を決定する
            //    RoleValue finalReachRoleValue = scenario.IsWinIfReach ?
            //        scenario.FirstRoleset.ReachStatus.Value.ReachedRole :
            //        scenario.FirstRoleset.ReachStatus.Value.ReachedRole.NextRoleValue;
            //    production = new Production(firstRoleSet, finalReachRoleValue);
            //}
            //else
            //{ //リーチではない
            //    production = new Production(firstRoleSet);
            //}

            return m_slotDriver.ControlBy(production);
        }
    }
}