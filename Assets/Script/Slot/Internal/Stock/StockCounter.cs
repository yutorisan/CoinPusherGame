using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;
using System;

namespace MedalPusher.Slot.Stock
{

    public interface IReadOnlyObservableStockCount
    {
        IReadOnlyReactiveProperty<int> StockCount { get; }
    }
    public interface IStockCounter
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
        void SpendStock();
    }
    /// <summary>
    /// スロットのストックを数える
    /// </summary>
    public class StockCounter : SerializedMonoBehaviour, IStockCounter, IReadOnlyObservableStockCount
    {
        [SerializeField]
        private IObservableMedalChecker m_medalChecker;

        /// <summary>
        /// 現在のストック数
        /// </summary>
        private ReactiveProperty<int> m_stockCount = new ReactiveProperty<int>(0);

        // Start is called before the first frame update
        void Start()
        {
            //メダルが入ったらストックをインクリメント
            m_medalChecker.Checked
                          .Subscribe(_ =>　++m_stockCount.Value);
        }

        public void SpendStock()
        {
            if (IsSpendable) --m_stockCount.Value;
            else throw new InvalidOperationException("ストックを消費しようとしましたが、消費できるストックが存在しません");
        }

        public IObservable<Unit> StockSupplied =>
            m_stockCount.Pairwise()
                        .Where(pair => pair.Previous <= 0 && pair.Current > 0) //ストックがない状態から、ストックがある状態に変化した
                        .AsUnitObservable();
                                                              
        public bool IsSpendable => m_stockCount.Value > 0;

        public IReadOnlyReactiveProperty<int> StockCount => m_stockCount;
    }
}