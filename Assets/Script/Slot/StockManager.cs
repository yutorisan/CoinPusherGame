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

		private int m_stockCount;

        public void AddStock()
        {
            //ストックが溜まっていなかった場合はスロットを回転させる
			if (++StockCount == 1)
			{
				m_slot.Roll();
			};
        }

        public int StockCount
        {
			get => m_stockCount;
            set
            {
				m_stockCount = value;
				m_StockText.text = m_stockCount.ToString();
            }
        }

		// Start is called before the first frame update
		void Start()
		{
			m_slot = GameObject.Find("Slot").GetComponent<ISlot>();
			m_slot.ObservableSlotStatus.Where(s => s == SlotStatus.Idol)
									   .Where(_ => StockCount > 0)
									   .Subscribe(_ => m_slot.Roll());
		}
	}

}

