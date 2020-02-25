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
				TryRolling();
			};
        }

        public int StockCount
        {
			get => m_stockCount;
            set
            {
				m_stockCount = value;
				m_StockText.text = "ストック：" + m_stockCount;
            }
        }

		// Start is called before the first frame update
		void Start()
		{
			m_slot = GameObject.Find("Slot").GetComponent<ISlot>();
			m_slot.Status.Where(s => s == SlotStatus.Idol)
					     .Where(_ => StockCount > 0)
				   	     .Subscribe(_ => TryRolling());
		}

        private void TryRolling()
        {
            if (m_slot.TryRoll() == TryRollReturn.Accept)
            {
                --StockCount;
            }
        }
	}

}

