﻿using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Input;
using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using UniRx;
using UnityEngine;
using Zenject;

public interface IObservableMedalInventory
{
    /// <summary>
    /// インベントリ内のメダル枚数を取得します。
    /// </summary>
    IObservable<int> ObservableMedalInventoryCount { get; }
}

public class MedalInventory : MonoBehaviour, IObservableMedalInventory
{
    /// <summary>
    /// メダルの獲得通知を受け取る
    /// </summary>
    [Inject]
    private IObservableItemChecker<IMedal> m_medalChecker;
    /// <summary>
    /// メダル投入時に払出し司令を行う
    /// </summary>
    [Inject]
    private IMedalPayoutOperator m_medalPayoutOperator;

    /// <summary>
    /// インベントリ内のメダル枚数
    /// </summary>
    [SerializeField]
    private IntReactiveProperty m_inventoryMedalCount = new IntReactiveProperty();

    /// <summary>
    /// インスペクタのメダルからフィールドに支払う
    /// </summary>
    private void PayoutFromInventory()
    {
        //メダルが1枚以上あったら支払う
        if(m_inventoryMedalCount.Value > 0)
        {
            m_medalPayoutOperator.Payout(1);
            --m_inventoryMedalCount.Value;
        }
    }

    private void Start()
    {
        //獲得メダルを購読してインベントリに追加
        m_medalChecker.ItemChecked
                      .Subscribe(medal => m_inventoryMedalCount.Value += medal.Value);
        //メダル投入コマンドを受け取ったらメダルを投入
        GameCommandFormatter.Instance
                            .ObservableGameCommand
                            .Where(cmd => cmd == GameCommand.InputInspectorMedal)
                            .Subscribe(_ => PayoutFromInventory());

    }


    public IObservable<int> ObservableMedalInventoryCount => m_inventoryMedalCount.AsObservable();


}
