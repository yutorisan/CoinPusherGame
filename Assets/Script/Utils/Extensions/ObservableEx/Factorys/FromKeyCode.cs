using System;
using UniRx;
using UnityEngine;

namespace MedalPusher.Utils
{
    public static partial class ObservableEx
    {
        /// <summary>
        /// 特定の<see cref="KeyCode"/>の入力通知を<see cref="IObservable{Unit}"/>で受け取ります
        /// </summary>
        /// <param name="key">通知を受け取る<see cref="KeyCode"/></param>
        /// <returns></returns>
        public static IObservable<Unit> FromKeyCode(KeyCode key) =>
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKey(key))
                      .AsUnitObservable();
    }
}