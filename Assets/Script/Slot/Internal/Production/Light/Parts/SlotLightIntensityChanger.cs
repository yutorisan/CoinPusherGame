using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Diagnostics;

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
            //それを防ぐため、Idolが発行されてから5フレーム以内に次のステータス変更通知が来なければ、Idolを通すようにし、
            //5フレーム以内にステータス変化があった場合はIdol状態通知は無視する

            //Idolの状態発行通知
            var idol = observableStatus
                .Where(s => s == SlotProductionStatus.Idol)
                //Idol通知が来てから5フレーム以内に次の通知が来なかったら、Idolを通す
                //5フレーム以内にIdol以外の次の通知が来たら、Idolの通知を遮断する
                .SelectMany(_ => observableStatus.TimeoutFrame(5)
                                                 //5フレーム以内に次の値が来なければIdolを発行する
                                                 .Catch((TimeoutException e) => Observable.Return(SlotProductionStatus.Idol)) 
                                                 .First())
                .Where(s => s == SlotProductionStatus.Idol);

            //Idol以外の状態発行通知
            var other = observableStatus
                .Where(s => s != SlotProductionStatus.Idol);

            //ステータスに応じてライト強度を変更
            idol.Merge(other)
                .Select(s => IntensityTable[s])
                .Subscribe(intensity => light.intensity = intensity);

        }
    }
}