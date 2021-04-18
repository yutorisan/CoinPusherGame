using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MedalPusher.Slot;
using UnityEngine;
using UnityUtility;
using UnityUtility.Enums;
using UniRx;

namespace MedalPusher.Slot.Internal.Productions
{
    /// <summary>
    /// ライトの照射方向を制御する
    /// </summary>
    public class SlotLightLookAtChanger
    {
        private readonly IReadOnlyDictionary<SlotProductionStatus, Action> lookAtSequenceTable;
        /// <summary>
        /// ライトの回転照射の半径
        /// </summary>
        private readonly float circleRadius;
        /// <summary>
        /// 回転照射円の中心座標
        /// </summary>
        private readonly Vector3 circleCenterPoint;
        /// <summary>
        /// 現在の照射円上の角度
        /// </summary>
        private Angle nowIrradiationAngle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="light">ライト</param>
        /// <param name="observableStatus">ステータスの変更通知</param>
        /// <param name="circleCenterPoint">回転照射する際の回転中心座標</param>
        /// <param name="radius">回転照射する際の回転半径</param>
        /// <param name="lr">右のライトか左のライトか</param>
        public SlotLightLookAtChanger(UnityEngine.Light light,
                                      IObservable<SlotProductionStatus> observableStatus,
                                      Vector3 circleCenterPoint,
                                      float radius,
                                      LeftRight lr)
        {
            //初期照射座標をセット
            light.transform.LookAt(circleCenterPoint);
            //ライトの照射座標を回転移動させるシーケンス
            Tween tween = DOTween.To(AnglePlugin.Instance,
                                     () => nowIrradiationAngle,
                                     angle =>
                                     {
                                         nowIrradiationAngle = angle;
                                         light.transform.LookAt(circleRadius * angle.Point.AddZ() + this.circleCenterPoint);
                                     },
                                     lr == LeftRight.Left ? Angle.Round : -Angle.Round,
                                     1f)
                                 .SetRelative()
                                 .SetEase(Ease.Linear)
                                 .SetLoops(-1)
                                 .OnPause(() => light.transform.LookAt(circleCenterPoint));

            this.circleCenterPoint = circleCenterPoint;
            this.circleRadius = radius;

            //リーチと当たったときだけ、ライトの照射先を回転させる
            lookAtSequenceTable = new Dictionary<SlotProductionStatus, Action>()
            {
                [SlotProductionStatus.Idol] = () => tween.Pause(),
                [SlotProductionStatus.Rolling] = () => tween.Pause(),
                [SlotProductionStatus.Reaching] = () => tween.Play(),
                [SlotProductionStatus.Winning] = () => tween.Play(),
            };

            observableStatus.Select(s => lookAtSequenceTable[s])
                            .Subscribe(act => act());
        }
    }
}