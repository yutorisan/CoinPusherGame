using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityUtility;

public interface IMedalChecker
{
    /// <summary>
    /// チェッカーがメダルを検知したときの通知
    /// </summary>
    /// <value></value>
    IObservable<Unit> ObservableMedalChecked{ get; }
}

[Serializable]
public class SerializableIMedalChecker : SerializeInterface<IMedalChecker> { }
[Serializable]
public class SerializableIMedalCheckerCollection : SerializeInterfaceCollection<IMedalChecker> { }

/// <summary>
/// メダルが通過したら通知する
/// </summary>
public class MedalChecker : MonoBehaviour, IMedalChecker
{
    [SerializeField]
    private bool m_destroyWhenChecked;

    private Subject<Unit> m_medalChecked = new Subject<Unit>();

    public IObservable<Unit> ObservableMedalChecked => m_medalChecked.AsObservable();

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Medal")
        {
            m_medalChecked.OnNext(Unit.Default);

            if (m_destroyWhenChecked) Destroy(other.gameObject);
        }    
    }
}
