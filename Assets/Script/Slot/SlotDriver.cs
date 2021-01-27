using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

namespace MedalPusher.Slot
{
    public interface ISlotDriver
    {
        /// <summary>
        /// 指定した演出に従ってスロットを制御します。
        /// </summary>
        /// <param name="production">演出</param>
        /// <returns>スロットの制御完了通知</returns>
        UniTask ControlBy(Production production);
    }
    public class SlotDriver : MonoBehaviour, ISlotDriver
    {
        [Inject(Id = "Left")]
        private IReelDriver m_leftReelDriver;
        [Inject(Id = "Middle")]
        private IReelDriver m_middleReelDriver;
        [Inject(Id = "Right")]
        private IReelDriver m_rightReelDriver;

        public UniTask ControlBy(Production production)
        {
            var leftReelControlTask = m_leftReelDriver.ControlBy(production.LeftPart);
            var middleReelControlTask = m_middleReelDriver.ControlBy(production.MiddlePart);
            var rightReelControlTask = m_rightReelDriver.ControlBy(production.RightPart);

            return UniTask.WhenAll(leftReelControlTask, middleReelControlTask, rightReelControlTask);
        }
    }
}