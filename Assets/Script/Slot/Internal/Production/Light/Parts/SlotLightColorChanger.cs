using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;
using DG.Tweening;
using MedalPusher.Utils;

namespace MedalPusher.Slot.Internal.Productions
{
    /// <summary>
    /// スロット用ライトの色を調整する
    /// </summary>
    public class SlotLightColorChanger
    {
        private readonly IReadOnlyDictionary<SlotProductionStatus, Action> actionTable;
        private readonly SequenceSwitcher<SlotProductionStatus> sequenceSwitcher = new SequenceSwitcher<SlotProductionStatus>();

        public SlotLightColorChanger(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            //当たったときに再生するSequenceを登録
            sequenceSwitcher.Register(SlotProductionStatus.Winning,
                                 DOTween.Sequence()
                                        .Append(light.DOColor(new Color(0, 0, 1), .5f))
                                        .Append(light.DOColor(new Color(0, 1, 0), .5f))
                                        .Append(light.DOColor(new Color(1, 0, 0), .5f))
                                        .SetLoops(-1));

            //ステータスによって実行する処理（ライト色変更）
            actionTable = new Dictionary<SlotProductionStatus, Action>()
            {
                [SlotProductionStatus.Idol] = () => { },
                [SlotProductionStatus.Rolling] = () => light.color = Color.white,
                [SlotProductionStatus.Reaching] = () => light.color = Color.yellow,
                [SlotProductionStatus.Winning] = () => { },
            };

            //ステータス変更に応じてライトを制御
            observableStatus.Subscribe(status => sequenceSwitcher.SwitchTo(status));
            observableStatus.Select(status => actionTable[status])
                            .Subscribe(action => action());
        }
    }
}