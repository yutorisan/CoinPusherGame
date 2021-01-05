using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの抽選を開始させる
    /// </summary>
    public interface ISlotStarter
    {
        /// <summary>
        /// スロット開始
        /// </summary>
        void SlotStart();
    }
    /// <summary>
    /// スロットの抽選を開始させる
    /// </summary>
    public class SlotStarter : MonoBehaviour, ISlotStarter
    {
        [Inject]
        private ISlotRoleDeterminer _roleDeterminer;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SlotStart()
        {
            throw new NotImplementedException();
        }
    }
}