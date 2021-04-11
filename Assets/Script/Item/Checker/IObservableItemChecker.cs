using System;
using System.ComponentModel;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IObservableItemChecker<TItem> where TItem : IFieldObject
    {
        /// <summary>
        /// アイテムを検出した
        /// </summary>
        IObservable<TItem> Checked { get; }
    }
    public interface IObservableInColliderCountableItemChecker<TItem> : IObservableItemChecker<TItem> where TItem : IFieldObject
    {
        /// <summary>
        /// Collider内にいるアイテムの数
        /// </summary>
        IObservable<int> InColliderCount { get; }
    }

    /// <summary>
    /// メダルの検出通知を受け取る
    /// </summary>
    public interface IObservableMedalChecker : IObservableItemChecker<IMedal> { }
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    public interface IObservableInColliderCountableMedalChecker : IObservableInColliderCountableItemChecker<IMedal> { }

}