using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    public interface ISlotDriver
    {
        /// <summary>
        /// 指定した演出に従ってスロットをコントロールします。
        /// </summary>
        /// <param name="production"></param>
        void ControlBy(Production production);
    }
    public class SlotDriver : MonoBehaviour, ISlotDriver
    {
        [Inject(Id = "Left")]
        private IReelDriver m_leftReelDriver;
        [Inject(Id = "Middle")]
        private IReelDriver m_middleReelDriver;
        [Inject(Id = "Right")]
        private IReelDriver m_rightReelDriver;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void ControlBy(Production production)
        {
            m_leftReelDriver.ControlBy(production.LeftPart);
            m_middleReelDriver.ControlBy(production.MiddlePart);
            m_rightReelDriver.ControlBy(production.RightPart);
        }
    }
}