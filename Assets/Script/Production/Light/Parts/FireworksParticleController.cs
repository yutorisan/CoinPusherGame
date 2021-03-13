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
                  .SelectMany(s => Observable.Range(0, particles.Count))
                  .SelectMany(index => Observable.Create<int>(observer =>
                  {
                      var disposable = new CancellationDisposable();
                      Task.Run(async () =>
                      {
                          await UniTask.Delay(random.Next(0, 1500));
                          observer.OnNext(index);
                      }, disposable.Token);
                      return disposable;
                  }))
                  .Subscribe(index => particles[index].Play());
        }
    }
}