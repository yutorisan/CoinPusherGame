using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UniRx;

/// <summary>
/// MedalCounter内で管理される各種メダル数の変化の購読を提供する
/// </summary>
public interface IObservableMedalCounter
{
    /// <summary>
    /// 投入したメダル数
    /// </summary>
    /// <value></value>
    IObservable<int> ObservableInputMedalCount{ get; }
    /// <summary>
    /// 獲得したメダル数
    /// </summary>
    /// <value></value>
    IObservable<int> ObservableWonMedalCount{ get; }
    /// <summary>
    /// 機械に回収されたメダル数
    /// </summary>
    /// <value></value>
    IObservable<int> ObservableFailedMedalCount{ get; }
}

/// <summary>
/// メダルの枚数に関する各種項目をカウントする
/// </summary>
public class MedalCounter : MonoBehaviour, IObservableMedalCounter
{
    [SerializeField]
    private SerializableIMedalChecker m_inputMedalChecker;
    [SerializeField]
    private SerializableIMedalChecker m_winMedalChecker;
    [SerializeField]
    private SerializableIMedalCheckerCollection m_failedMedalChecker;

    /// <summary>
    /// 投入したメダル枚数
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    private ReactiveProperty<int> m_inputMedalCount = new ReactiveProperty<int>();
    /// <summary>
    /// 獲得したメダル枚数
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    private ReactiveProperty<int> m_wonMedalCount = new ReactiveProperty<int>();
    /// <summary>
    /// 機械に回収されたメダル枚数
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    private ReactiveProperty<int> m_failedMedalCount = new ReactiveProperty<int>();

    public IObservable<int> ObservableInputMedalCount => m_inputMedalCount.AsObservable();

    public IObservable<int> ObservableWonMedalCount => m_wonMedalCount.AsObservable();

    public IObservable<int> ObservableFailedMedalCount => m_failedMedalCount.AsObservable();

    // Start is called before the first frame update
    void Start()
    {
        m_inputMedalChecker.Interface.ObservableMedalChecked.Subscribe(_ => ++m_inputMedalCount.Value);
        m_winMedalChecker.Interface.ObservableMedalChecked.Subscribe(_ => ++m_wonMedalCount.Value);
        foreach (var failedChecker in m_failedMedalChecker.InterfaceCollection)
        {
            failedChecker.ObservableMedalChecked.Subscribe(_ => ++m_failedMedalCount.Value);
        }

        print(m_inputMedalChecker.Interface.Equals(m_winMedalChecker.Interface));

    }
}
