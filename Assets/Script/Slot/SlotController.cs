using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MedalPusher.Slot
{
	public interface ISlotController
	{
		void AddStock();
	}

	public class SlotController : MonoBehaviour, ISlotController
	{
		private IStockManager m_stockManager;
		private ISlot m_slot;

		public void AddStock() => m_stockManager.AddStock();




		// Start is called before the first frame update
		void Start()
		{
			m_stockManager = this.GetChildGameObject("StockManager").GetComponent<IStockManager>();
			m_slot = this.GetChildGameObject("Slot").GetComponent<ISlot>();
			m_stockManager.ObservableStockExpend.Subscribe(_ => m_slot.Roll());

		
		}

		// Update is called once per frame
		void Update()
		{
        
		}

	}

}

