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
		/// <summary>
		/// ストックを消費した通知を購読する
		/// </summary>
		IObservable<Unit> ObservableStockExpend { get; }
	}

	public class StockManager : MonoBehaviour, IStockManager
	{
		[SerializeField]
		private Text m_StockText;

		private Subject<Unit> m_StockExpendStream = new Subject<Unit>();
		private ReactiveProperty<int> m_stockCount = new ReactiveProperty<int>();


		public void AddStock() => ++m_stockCount.Value;

		public IObservable<Unit> ObservableStockExpend => m_StockExpendStream;

		// Start is called before the first frame update
		void Start()
		{
			m_stockCount.Subscribe(n => m_StockText.text = "ストック：" + n);
		}

		// Update is called once per frame
		void Update()
		{
			if(m_stockCount.Value > 0)
			{
				m_StockExpendStream.OnNext(Unit.Default);
			}
		}
	}

}

