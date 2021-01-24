using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;

namespace MedalPusher.Slot.Stock
{
    public class StockCounterView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_stockText;
        [Inject]
        private IObservableStockCount _stockCount;

        // Start is called before the first frame update
        void Start()
        {
            _stockCount.StockCount.Subscribe(stock => m_stockText.text = $"Stocks:{stock}");
        }
    }
}