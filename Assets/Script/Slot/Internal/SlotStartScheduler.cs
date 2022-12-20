using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Internal.Stock;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace MedalPusher.Slot.Internal
{
    /// <summary>
    /// ストック数とスロットの状況を監視して、スロットを開始するタイミングを制御する
    /// </summary>
    internal class SlotStartScheduler : MonoBehaviour, ISlotResultSubmitter
    {
        [Inject]
        private ISlotStarter slotStarter;
        [Inject]
        private IStockList stockCounter;
        /// <summary>
        /// スロットの結果を発行するためのSubject
        /// </summary>
        private readonly Subject<IReadOnlySlotResult> slotResultSubject = new Subject<IReadOnlySlotResult>();

        void Start()
        {
            //スロット回転タスク
            UniTask<ISlotResult> slotTask = UniTask.FromResult<ISlotResult>(default);

            //スロットの開始条件が整ったらスロットを回転させる
            //★Schedulerを指定しているのは、StockCounterからのストック追加通知を受けてViewが数字を更新するよりも先に、
            //★スロット開始指令が送られてしまってViewの数字がおかしくなってしまうのを防ぐため。
            //★MainThreadEndOfFrameを指定することで、StockCounterからの他の通知よりも後にスロット開始指令が送られるようになる。
            stockCounter.StockSupplied.ObserveOn(Scheduler.MainThreadEndOfFrame)        //ストックが供給された
                        .Where(_ => slotTask.Status == UniTaskStatus.Succeeded)         //かつ前回のスロット回転ステータスが完了している
                        .Merge(slotResultSubject.Where(_ => stockCounter.IsSpendable)   //または、前のスロットの回転が終了して、かつまだストックがある
                                                .AsUnitObservable())   
                        .Subscribe(async _ =>                                           //ならば、ストックを消費してスロットの回転を開始させる
                        {
                            //ストックを消費してスロットの回転を開始させる
                            var stockLevelInfo = stockCounter.SpendStock();
                            slotTask = slotStarter.StartAsync().Preserve();  //Preserveはawait後にStatus参照するのに必要

                            //スロットの回転が完了したら、結果を発行する
                            var result = await slotTask;
                            result.SetStockLevelInfo(stockLevelInfo);
                            slotResultSubject.OnNext(result);
                        });

        }

        public IObservable<IReadOnlySlotResult> ObservableSlotResult => slotResultSubject.AsObservable();

    }

    /// <summary>
    /// StockCounterへのアクセス定義
    /// </summary>
    internal interface IStockList
    {
        /// <summary>
        /// ストックが0の状態からストックが供給されたときに通知される
        /// </summary>
        IObservable<Unit> StockSupplied { get; }
        /// <summary>
        /// ストックを消費可能か
        /// </summary>
        bool IsSpendable { get; }
        /// <summary>
        /// ストックを1つ消費する
        /// </summary>
        StockLevelInfo SpendStock();
    }
    /// <summary>
    /// スロットの回転を開始させる
    /// </summary>
    internal interface ISlotStarter
    {
        UniTask<ISlotResult> StartAsync();
    }
}