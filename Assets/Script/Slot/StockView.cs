using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using MedalPusher.Slot;
using UniRx;
using System;

public class StockView : MonoBehaviour
{
    [SerializeField]
    private SerializableIObservableStockCount m_stockCount;

    // Start is called before the first frame update
    void Start()
    {
        var text = this.gameObject.GetComponent<Text>();
        m_stockCount.Interface.ObservableStockCount.SubscribeToText(text, count => $"ストック：{count}");
    }

    [Serializable]
    private class SerializableIObservableStockCount : SerializeInterface<IObservableStockCount> { }
}
