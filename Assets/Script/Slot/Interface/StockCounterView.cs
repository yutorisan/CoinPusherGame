using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using MedalPusher.Slot.Internal.Stock;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot.Stock
{
    /// <summary>
    /// スロットのストック数に対するView
    /// </summary>
    public class StockCounterView : MonoBehaviour
    {
        [SerializeField, Required]
        private TextMeshProUGUI stockText;
        [Inject]
        private IObservableStockCount stockCount;

        void Start()
        {
            //ストックを監視してTextMeshProに反映
            stockCount.StockCount.Subscribe(stock => stockText.text = stock.ToString());
        }
    }
}