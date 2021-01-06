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
    /// <summary>
    /// ストック数の変化を提供する
    /// </summary>
    public interface IObservableStockCount
    {
        IReadOnlyReactiveProperty<int> Stock { get; }
    }
    /// <summary>
    /// スロットのストックを数える
    /// </summary>
    public class StockCounter : SerializedMonoBehaviour, IObservableStockCount
    {
        [SerializeField]
        private IObservableMedalChecker m_medalChecker;

        /// <summary>
        /// 現在のストック数
        /// </summary>
        private ReactiveProperty<int> m_stock = new ReactiveProperty<int>();

        // Start is called before the first frame update
        void Start()
        {
            //メダルが入ったらストックをインクリメント
            m_medalChecker.Checked.Subscribe(_ => ++m_stock.Value);
        }

        public IReadOnlyReactiveProperty<int> Stock => m_stock;
    }
}