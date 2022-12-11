using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityUtility.Linq.Core;
using Zenject;

namespace MedalPusher.Slot.Internal.Stock
{
    public class IndicatorController : MonoBehaviour
    {
        [Inject]
        private IObservableStockList stockList;
        [SerializeField]
        private GameObject indicatorSet;

        private IReadOnlyList<IIndicator> indicators;

        void Start()
        {
            indicators = indicatorSet.GetComponentsInChildren<IIndicator>().ToList();

            stockList.ObserveStockLevelChanged
                     .Merge(stockList.ObserveStockAdd)
                     .Subscribe(changeInfo => indicators[changeInfo.Index].Indicate(changeInfo.Value.stockImage));
            stockList.ObserveStockSpend
                     .First() // ストック消費ごとに区切る 後述のselectのindexを消費ごとにリセットさせるため
                     .SelectMany(_ => stockList.StockLevelList)
                     .Select((item, index) => item.WithIndex(index))
                     .Repeat()
                     .Subscribe(info => indicators[info.Index].Indicate(info.Value.stockImage));
        }
    }
}