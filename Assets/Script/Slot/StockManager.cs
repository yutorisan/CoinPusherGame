using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UnityEngine.Assertions.Must;

namespace MedalPusher.Slot
{

	public interface IObservableStockCount
	{
		/// <summary>
		/// スロットストック数を購読します。
		/// </summary>
		/// <value></value>
		IObservable<int> ObservableStockCount{ get; }
    }

	public class StockManager : MonoBehaviour, IObservableStockCount
	{
		[SerializeField]
		private SerializableISlot m_slot;
		[SerializeField]
		private SerializableIMedalChecker m_medalChecker;

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

		// Start is called before the first frame update
		void Start()
		{
			m_medalChecker.Interface.ObservableMedalChecked.Subscribe(_ => ++m_stockCount.Value);

			ISlot slot = m_slot.Interface;
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

