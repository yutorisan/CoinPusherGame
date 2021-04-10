using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using MedalPusher.Slot.Internal.Stock;

namespace MedalPusher.Slot.Stock
{
    /// <summary>
    /// スロットのストック数に対するView
    /// </summary>
    public class StockCounterView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_stockText;
        [Inject]
        private IReadOnlyObservableStockCount _stockCount;

        void Start()
        {
            //ストックを監視してTextMeshProに反映
            _stockCount.StockCount.Subscribe(stock => m_stockText.text = stock.ToString());
        }
    }
}