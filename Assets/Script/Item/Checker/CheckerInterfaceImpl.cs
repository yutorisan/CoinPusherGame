using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// メダルの検出通知を受け取る
    /// </summary>
    public interface IObservableMedalChecker : IObservableItemChecker<IMedal> { }
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    public interface IObservableFieldItemChecker : IObservableItemChecker<IFieldItem> { }
}
