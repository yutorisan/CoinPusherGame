using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットのステータスを購読可能
    /// </summary>
    public interface IObservableSlotStatus
    {
        IReadOnlyReactiveProperty<SlotStatus> Status { get; }
    }
    /// <summary>
    /// スロット
    /// </summary>
    public class Slot : SerializedMonoBehaviour, IObservableSlotStatus
    {
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_reftReel;
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_centerReel;
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_rightReel;

        public IReadOnlyReactiveProperty<SlotStatus> Status => Observable.Return(SlotStatus.Idol).ToReactiveProperty();


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum SlotStatus
    {
        /// <summary>
        /// 稼働していない
        /// </summary>
        Idol,
        /// <summary>
        /// 抽選中
        /// </summary>
        Rolling
    }
}