using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Internal.Stock;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot.Interface
{
    /// <summary>
    /// 現在のストックの状況をユーザーに提示する
    /// </summary>
    public class StockView : MonoBehaviour
    {
        [Inject]
        private IObservableStockList stockList;

        // Start is called before the first frame update
        void Start()
        {
            
        }
    }
}