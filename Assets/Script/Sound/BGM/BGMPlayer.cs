using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Lottery;
using MedalPusher.Slot;
using MedalPusher.Slot.Internal;
using MedalPusher.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Sound
{
    /// <summary>
    /// BGMの再生を制御する
    /// </summary>
    public class BGMPlayer : MonoBehaviour
    {
        [Inject]
        private IReadOnlyObservableSlotProdctionStatus slotStatus;
        [Inject]
        private IObservableLotteryStatus lotteryStatus;

        [SerializeField]
        private AudioClip normalBGM;
        [SerializeField]
        private AudioClip slotBGM;
        [SerializeField]
        private AudioClip slotReachBGM;
        [SerializeField]
        private AudioClip slotWinAudio;
        [SerializeField]
        private AudioClip lotteryBGM;


        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            //スロットのステータスによってBGMを切り替え
            slotStatus.ProductionStatus.Stabilize(TimeSpan.FromMilliseconds(100), SlotProductionStatus.Idol).Subscribe(status =>
            {
                var audio = status switch
                {
                    Slot.SlotProductionStatus.Idol => normalBGM,
                    Slot.SlotProductionStatus.Rolling => slotBGM,
                    Slot.SlotProductionStatus.Reaching => slotReachBGM,
                    Slot.SlotProductionStatus.Winning => slotWinAudio,
                    _ => throw new System.NotImplementedException(),
                };
                SwitchBGM(audio);
            });
        }

        /// <summary>
        /// BGMを切り替える
        /// </summary>
        /// <param name="clip"></param>
        private void SwitchBGM(AudioClip clip)
        {
            audioSource.Pause();
            if (clip == null) return; //BGMが設定されていない場合は止めるだけ
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
