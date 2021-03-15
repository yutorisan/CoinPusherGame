using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MedalPusher.Slot;
using UnityEngine;
using UnityUtility;
using UnityUtility.Enums;
using UniRx;

namespace MedalPusher.Production.Light
{
    public class SlotLightLookAtChanger
    {
        private readonly SequenceSwitcher<SlotProductionStatus> m_sequenceSwitcher = new SequenceSwitcher<SlotProductionStatus>();
        private readonly IReadOnlyDictionary<SlotProductionStatus, Action> m_lookAtSequenceTable;
        /// <summary>
        /// ライトの回転照射の半径
        /// </summary>
        private readonly float m_circleRadius;
        /// <summary>
        /// 回転照射円の中心座標
        /// </summary>
        private readonly Vector3 m_circleCenterPoint;
        /// <summary>
        /// 現在の照射円上の角度
        /// </summary>
        private Angle m_nowIrradiationAngle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="light">ライト</param>
        /// <param name="observableStatus">ステータスの変更通知</param>
        /// <param name="circleCenterPoint">回転照射する際の回転中心座標</param>
        /// <param name="radius">回転照射する際の回転半径</param>
        public SlotLightLookAtChanger(UnityEngine.Light light,
                                      IObservable<SlotProductionStatus> observableStatus,
                                      Vector3 circleCenterPoint,
                                      float radius,
                                      LeftRight lr)
        {
            light.transform.LookAt(circleCenterPoint);

            Tween tween = DOTween.To(AnglePlugin.Instance,
                                     () => m_nowIrradiationAngle,
                                     angle =>
                                     {
                                         m_nowIrradiationAngle = angle;
                                         light.transform.LookAt(m_circleRadius * angle.Point.AddZ() + m_circleCenterPoint);
                                     },
                                     lr == LeftRight.Left ? Angle.Round : -Angle.Round,
                                     1f)
                                 .SetRelative()
                                 .SetEase(Ease.Linear)
                                 .SetLoops(-1)
                                 .OnPause(() => light.transform.LookAt(circleCenterPoint));

            m_circleCenterPoint = circleCenterPoint;
            m_circleRadius = radius;
            m_lookAtSequenceTable = new Dictionary<SlotProductionStatus, Action>()
            {
                [SlotProductionStatus.Idol] = () => tween.Pause(),
                [SlotProductionStatus.Rolling] = () => tween.Pause(),
                [SlotProductionStatus.Reaching] = () => tween.Play(),
                [SlotProductionStatus.Winning] = () => tween.Play(),
            };

            observableStatus.Select(s => m_lookAtSequenceTable[s])
                            .Subscribe(act => act());
        }
    }
}