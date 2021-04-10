using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using System;
using MedalPusher.Slot;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace MedalPusher.Slot.Internal.Productions
{
    /// <summary>
    /// 当たったときの花火パーティクルエフェクトを制御する
    /// </summary>
    public class FireworksParticleController 
    {
        public FireworksParticleController(IReadOnlyList<ParticleSystem> particles, IObservable<SlotProductionStatus> status)
        {
            //当たった瞬間から1500ms以内のランダムなタイミングで花火を何個か打ち上げる
            status.Where(s => s == SlotProductionStatus.Winning)
                  .SelectMany(_ => ObservableEx.RandomTiming(0, 1500, particles.Count))
                  .Subscribe(index => particles[index].Play());
        }
    }
}

