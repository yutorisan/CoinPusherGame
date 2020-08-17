using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MedalPusher.Medal;
using MedalPusher.Slot;
using UnityUtility;
using System;

public class SlotChecker : MonoBehaviour
{
    [SerializeField]
    private SerializableIStockManager m_stockManager;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<ObservableTriggerTrigger>().OnTriggerEnterAsObservable()
            .Where(col => col.tag == "Medal")
            .Subscribe(_ => m_stockManager.Interface.AddStock());
    }

    [Serializable]
    private class SerializableIStockManager : SerializeInterface<IStockManager> { }
}
