using UniRx;
using System;

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
        void Add();
    }
    /// <summary>
    /// スロットのストック数を取得・購読できる
    /// </summary>
    public interface IReadOnlyObservableStockCount
    {
        /// <summary>
        /// スロットのストック数を取得・購読する
        /// </summary>
        IReadOnlyReactiveProperty<int> StockCount { get; }
    }

    /// <summary>
    /// スロットのストックを数える
    /// </summary>
    internal class StockCounter : IStockCounter, IStockAdder, IReadOnlyObservableStockCount
    {
        /// <summary>
        /// 現在のストック数
        /// </summary>
        private ReactiveProperty<int> m_stockCount = new ReactiveProperty<int>(0);

        public void Spend()
        {
            if (IsSpendable) --m_stockCount.Value;
            else throw new InvalidOperationException("ストックを消費しようとしましたが、消費できるストックが存在しません");
        }

        public void Add()
        {
            ++m_stockCount.Value;
        }

        public IObservable<Unit> Supplied =>
            m_stockCount.Pairwise()
                        .Where(pair => pair.Previous <= 0 && pair.Current > 0) //ストックがない状態から、ストックがある状態に変化した
                        .AsUnitObservable()
                        .Share();

        public bool IsSpendable => m_stockCount.Value > 0;

        public IReadOnlyReactiveProperty<int> StockCount => m_stockCount;
    }
}