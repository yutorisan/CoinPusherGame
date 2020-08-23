using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public interface IMedalChecker
{
    /// <summary>
    /// チェッカーがメダルを検知したときの通知
    /// </summary>
    /// <value></value>
    IObservable<Unit> ObservableMedalChecked{ get; }
}

/// <summary>
/// メダルが通過したら通知する
/// </summary>
public class MedalChecker : MonoBehaviour, IMedalChecker
{
    private Subject<Unit> m_medalChecked = new Subject<Unit>();

    public IObservable<Unit> ObservableMedalChecked => m_medalChecked.AsObservable();

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Medal")
        {
            m_medalChecked.OnNext(Unit.Default);
        }    
    }
}
