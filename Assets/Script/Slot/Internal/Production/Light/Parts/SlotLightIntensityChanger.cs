using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;

namespace MedalPusher.Production.Light
{
    /// <summary>
    /// ライトの強度を調整する
    /// </summary>
    public class SlotLightIntensityChanger
    {
        private static readonly IReadOnlyDictionary<SlotProductionStatus, float> IntensityTable = new Dictionary<SlotProductionStatus, float>()
        {
            [SlotProductionStatus.Idol] = 0f,
            [SlotProductionStatus.Rolling] = 1f,
            [SlotProductionStatus.Reaching] = 1.5f,
            [SlotProductionStatus.Winning] = 2f,
        };

        public SlotLightIntensityChanger(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            observableStatus.Select(s => IntensityTable[s])
                            .DistinctUntilChanged()
                            .Subscribe(intensity => light.intensity = intensity);
        }
    }
}