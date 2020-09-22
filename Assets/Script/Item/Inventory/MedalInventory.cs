using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

public class MedalInventory
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



}
