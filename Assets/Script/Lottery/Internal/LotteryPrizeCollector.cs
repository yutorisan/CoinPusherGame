using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選の結果を発行する
    /// </summary>
    public interface ILotteryResultSubmitter
    {
        IObservable<LotteryPrizeInfo> LotteryResult { get; }
    }
    /// <summary>
    /// 抽選の景品を投入する口。
    /// ここに放り込んでおけば適当なタイミングでフィールドに渡してくれる
    /// </summary>
    internal interface ILotteryPrizeInsertionSlot
    {
        /// <summary>
        /// 手に入れた景品を投入する
        /// </summary>
        /// <param name="prizeInfo"></param>
        void InsertPrize(LotteryPrizeInfo prizeInfo);
    }
    /// <summary>
    /// 各<see cref="LotteryPocket"/>のPrizeをまとめて受け取る
    /// </summary>
    internal class LotteryPrizeCollector : ILotteryPrizeInsertionSlot, ILotteryResultSubmitter
    {
        private readonly Subject<LotteryPrizeInfo> lotteryResultSubject = new Subject<LotteryPrizeInfo>();

        public IObservable<LotteryPrizeInfo> LotteryResult => lotteryResultSubject.AsObservable();

        public void InsertPrize(LotteryPrizeInfo prizeInfo)
        {
            lotteryResultSubject.OnNext(prizeInfo);
        }
    }
}