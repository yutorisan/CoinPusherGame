using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using System;
using Cysharp.Threading.Tasks;

namespace MedalPusher.Utils
{
    public static partial class ObservableEx
    {
        /// <summary>
        /// ランダムなタイミングで値を発行するシーケンスを生成します。
        /// </summary>
        /// <param name="minTimingMillisecond">タイミングの最小値</param>
        /// <param name="maxTimingMillisecond">タイミングの最大値</param>
        /// <param name="count">発行回数</param>
        /// <returns></returns>
        public static IObservable<int> RandomTiming(int minTimingMillisecond, int maxTimingMillisecond, int count)
        {
            //return RandomTiming(TimeSpan.FromMilliseconds(minTimingMillisecond), TimeSpan.FromMilliseconds(maxTimingMillisecond), count);
            return Observable.Range(0, count)
                             .SelectMany(i => Observable.Timer(TimeSpan.FromMilliseconds(UnityEngine.Random.Range(minTimingMillisecond, maxTimingMillisecond))),
                                         (i, _) => i);
        }
        /// <summary>
        /// ランダムなタイミングで値を発行するシーケンスを生成します。
        /// </summary>
        /// <param name="minTiming">タイミングの最小値</param>
        /// <param name="maxTiming">タイミングの最大値</param>
        /// <param name="count">発行回数</param>
        /// <returns></returns>
        public static IObservable<int> RandomTiming(TimeSpan minTiming, TimeSpan maxTiming, int count)
        {
            return RandomTiming((int)minTiming.TotalMilliseconds, (int)maxTiming.TotalMilliseconds, count);
        }
    }
}