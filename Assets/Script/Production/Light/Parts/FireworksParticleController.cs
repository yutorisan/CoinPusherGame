using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using System;
using MedalPusher.Slot;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace MedalPusher.Production
{
    public class FireworksParticleController 
    {
        public FireworksParticleController(IReadOnlyList<ParticleSystem> particles, IObservable<SlotProductionStatus> status)
        {
            System.Random random = new System.Random();

            status.Where(s => s == SlotProductionStatus.Winning)
                  .SelectMany(_ => ObservableEx.RandomTiming(0, 1500, 3))
                  .Subscribe(index => particles[index].Play());
        }
    }
}

