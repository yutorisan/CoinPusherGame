using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;
using DG.Tweening;

namespace MedalPusher.Production.Light
{
    /// <summary>
    /// スロット用ライトの色を調整する
    /// </summary>
    public class SlotLightColorChanger
    {
        private readonly IReadOnlyDictionary<SlotProductionStatus, Action> m_actionTable;
        private readonly IReadOnlyDictionary<SlotProductionStatus, Color> m_colorTable;
        private readonly SequenceSwitcher<SlotProductionStatus> m_sequenceSwitcher = new SequenceSwitcher<SlotProductionStatus>();

        public SlotLightColorChanger(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            m_sequenceSwitcher.Add(SlotProductionStatus.Winning,
                                   DOTween.Sequence()
                                          .Append(light.DOColor(new Color(0, 0, 1), .5f))
                                          .Append(light.DOColor(new Color(0, 1, 0), .5f))
                                          .Append(light.DOColor(new Color(1, 0, 0), .5f))
                                          .SetLoops(-1));

            m_actionTable = new Dictionary<SlotProductionStatus, Action>()
            {
                [SlotProductionStatus.Idol] = () => { },
                [SlotProductionStatus.Rolling] = () => light.color = Color.white,
                [SlotProductionStatus.Reaching] = () => light.color = Color.yellow,
                [SlotProductionStatus.Winning] = () => { },
            };

            observableStatus.Subscribe(status => m_sequenceSwitcher.SwitchTo(status));
            observableStatus.Select(status => m_actionTable[status])
                            .Subscribe(action => action());
        }
    }
}