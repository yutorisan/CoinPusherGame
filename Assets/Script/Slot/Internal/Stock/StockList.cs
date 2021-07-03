using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModestTree;
using UniRx;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.Linq;
using Zenject;

namespace MedalPusher.Slot.Internal.Stock
{
    /// <summary>
    /// スロットのストックを追加できる
    /// </summary>
    public interface IStockAdder
    {
        /// <summary>
        /// スロットのストックを追加する
        /// </summary>
        void AddStock();
    }
    /// <summary>
    /// スロットのストック数を取得・購読できる
    /// </summary>
    public interface IObservableStockCount
    {
        /// <summary>
        /// スロットのストック数を取得・購読する
        /// </summary>
        IReadOnlyReactiveProperty<int> StockCount { get; }
    }
    public interface IObservableStockList
    {
        /// <summary>
        /// ストックが追加されたときに通知を受ける
        /// </summary>
        IObservable<StockLevelInfo> ObserveStockAdd { get; }
        /// <summary>
        /// ストックを消費したときに通知を受ける
        /// </summary>
        IObservable<Unit> ObserveStockSpend { get; }
        /// <summary>
        /// いずれかのストックのレベルが変更されたときに通知を受ける
        /// </summary>
        IObservable<StockListLevelChangedEvent> ObserveStockLevelChanged { get; }
        /// <summary>
        /// 保持している<see cref="Stock"/>インスタンスの数
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// 所持中のストックのリスト
    /// </summary>
    internal class StockList : IStockList, IStockAdder, IObservableStockCount, IObservableStockList, IEnumerable<IStock>, IReadOnlyList<IStock>
    {
        /// <summary>
        /// 保持できる最大のストック数
        /// </summary>
        private static readonly int StockCapacity = 8;

        private readonly FixedCapacityReactiveQueue<IStock> stockQueue = new FixedCapacityReactiveQueue<IStock>(StockCapacity);
        private readonly IReadOnlyStockLevelSetting stockLevelSetting;

        public StockList(IReadOnlyStockLevelSetting setting) => stockLevelSetting = setting;

        public int Count => stockQueue.Count;

        public IObservable<StockLevelInfo> ObserveStockAdd =>
            stockQueue.ObserveEnqueue()
                      .Select(stock => GetStockLevelInfo(stock))
                      .Share();

        public IObservable<Unit> ObserveStockSpend =>
            stockQueue.ObserveDequeue()
                      .AsUnitObservable()
                      .Share();

        private readonly Subject<StockListLevelChangedEvent> levelChangedSubject = new Subject<StockListLevelChangedEvent>();
        public IObservable<StockListLevelChangedEvent> ObserveStockLevelChanged => levelChangedSubject.AsObservable();

        
        public IObservable<Unit> StockSupplied =>
            //ストックがEnqueueされて、ストック数が1の場合に、0状態からストックが供給されたとみなす
            stockQueue.ObserveEnqueue()
                      .Where(_ => Count == 1)
                      .AsUnitObservable();

        public bool IsSpendable => !stockQueue.IsEmpty();

        public IReadOnlyReactiveProperty<int> StockCount => stockQueue.ObserveCountChanged().ToReactiveProperty();

        public IStock this[int index]
        {
            get
            {
                if (stockQueue.Count <= index) throw new IndexOutOfRangeException();
                return stockQueue.ElementAt(index);
            }
        }

        public void AddStock()
        {
            //空きがあればストックインスタンスを新規追加
            if (stockQueue.HasVacancy)
            {
                stockQueue.Enqueue(new Stock());
            }
            else //空きがなければ
            {
                //最小レベルの最初のストックインスタンスを見つける
                var (stock, index) = stockQueue.WithIndex().MinByFirst(stock => stock.Value.Level);
                //そのストックをアップグレード
                stock.UpGrade();
                //レベルが変化したことを通知する
                levelChangedSubject.OnNext(new StockListLevelChangedEvent(index, GetStockLevelInfo(stock)));
            }
        }

        public StockLevelInfo SpendStock()
        {
            //ストックが空で消費できなければ例外を投げる
            if (!IsSpendable) throw new InvalidOperationException("ストックがないため消費できません");
            //消費可能なら消費する
            var level = stockQueue.Dequeue().Spend();
            //消費したストックのレベルの情報を返す
            return stockLevelSetting.StockLevelSettingList[level];
        }

        public IEnumerator<IStock> GetEnumerator() => stockQueue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private StockLevelInfo GetStockLevelInfo(IStock stock) => stockLevelSetting.StockLevelSettingList[stock.Level];
    }

    public readonly struct StockListLevelChangedEvent
    {
        public int Index { get; }
        public StockLevelInfo Stock { get; }

        public StockListLevelChangedEvent(int index, StockLevelInfo stock) => (Index, Stock) = (index, stock);
    }
}