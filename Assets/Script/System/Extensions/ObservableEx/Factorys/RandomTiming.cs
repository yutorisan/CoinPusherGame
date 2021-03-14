using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using System;
using Cysharp.Threading.Tasks;

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
        return RandomTiming(TimeSpan.FromMilliseconds(minTimingMillisecond), TimeSpan.FromMilliseconds(maxTimingMillisecond), count);
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
        System.Random random = new System.Random();
        return Observable.Range(0, count)
                         .SelectMany(i => UniTask.Delay(random.Next((int)minTiming.TotalMilliseconds,
                                                                    (int)maxTiming.TotalMilliseconds))
                                                 .ToObservable()
                                                 .Select(_ => i));

    }
}