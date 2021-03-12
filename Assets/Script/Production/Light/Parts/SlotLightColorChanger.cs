using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;

namespace MedalPusher.Production.Light
{
    /// <summary>
    /// スロット用ライトの色を調整する
    /// </summary>
    public class SlotLightColorChanger
    {
        private static readonly IReadOnlyDictionary<SlotProductionStatus, Color> ColorTable = new Dictionary<SlotProductionStatus, Color>()
        {
            [SlotProductionStatus.Idol] = Color.white,
            [SlotProductionStatus.Rolling] = Color.white,
            [SlotProductionStatus.Reaching] = Color.yellow,
            [SlotProductionStatus.Winning] = Color.red,
        };

        public SlotLightColorChanger(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            observableStatus.Select(s => ColorTable[s])
                            .DistinctUntilChanged()
                            .Subscribe(color => light.color = color);
        }
    }
}