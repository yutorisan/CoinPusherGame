using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Diagnostics;
using MedalPusher.Utils;

namespace MedalPusher.Slot.Internal.Productions
{
    /// <summary>
    /// ライトの強度を調整する
    /// </summary>
    public class SlotLightIntensityChanger
    {
        /// <summary>
        /// ステータスとライト強度のテーブル
        /// </summary>
        private static readonly IReadOnlyDictionary<SlotProductionStatus, float> IntensityTable = new Dictionary<SlotProductionStatus, float>()
        {
            [SlotProductionStatus.Idol] = 0f,
            [SlotProductionStatus.Rolling] = 1f,
            [SlotProductionStatus.Reaching] = 1.5f,
            [SlotProductionStatus.Winning] = 2f,
        };

        public SlotLightIntensityChanger(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            //残ストックがある状態でも、スロット終了時に一旦Idol状態となるため、スロットライトが一瞬消灯されてしまう
            //それを防ぐため、Idolが発行されてから100ms以内に次のステータス変更通知が来なければ、Idolを通すようにし、
            //100ms以内にステータス変化があった場合はIdol状態通知は無視する

            observableStatus
                .Stabilize(TimeSpan.FromMilliseconds(100), SlotProductionStatus.Idol)
                .Select(s => IntensityTable[s])
                .Subscribe(intensity => light.intensity = intensity);

        }
    }
}