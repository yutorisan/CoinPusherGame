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

	public class StockManager : MonoBehaviour, IStockManager
	{
		[SerializeField]
		private Text m_StockText;

		private ISlot m_slot;

		private Subject<Unit> m_StockExpendStream = new Subject<Unit>();
		private ReactiveProperty<int> m_stockCount = new ReactiveProperty<int>();


		public void AddStock() => ++m_stockCount.Value;

		// Start is called before the first frame update
		void Start()
		{
			m_stockCount.Subscribe(n => m_StockText.text = "ストック：" + n);
		}
	}

}

