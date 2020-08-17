using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

namespace MedalPusher.Slot
{
	public interface IStockManager
	{
		/// <summary>
		/// ストックを追加する
		/// </summary>
		void AddStock();
	}
	public interface IObservableStockCount
	{
		/// <summary>
		/// スロットストック数を購読します。
		/// </summary>
		/// <value></value>
		IObservable<int> ObservableStockCount{ get; }
    }

	public class StockManager : MonoBehaviour, IStockManager, IObservableStockCount
	{
		/// <summary>
		/// ストック残量をチェックする間隔
		/// </summary>
        private static readonly TimeSpan StockCheckInterval = TimeSpan.FromMilliseconds(500);

		/// <summary>
		/// ストック数
		/// </summary>
        private ReactiveProperty<int> m_stockCount = new ReactiveProperty<int>();

        public bool HasStock => m_stockCount.Value > 0;
        public IObservable<int> ObservableStockCount => m_stockCount.AsObservable();

        public void AddStock() => ++m_stockCount.Value;

		// Start is called before the first frame update
		void Start()
		{
			var slot = GameObject.Find("Slot").GetComponent<ISlot>();

			//スロットがアイドル状態、かつストックがある状態ならスロットを回してストックを減らす
            Observable.Interval(StockCheckInterval)
                      .Where(_ => slot.ObservableStatus.Value == SlotStatus.Idol && HasStock)
                      .Subscribe(_ => 
					  {
                          --m_stockCount.Value;
                          slot.Roll();
                      });
		}
	}

}

