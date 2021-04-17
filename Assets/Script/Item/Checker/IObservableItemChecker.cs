using System;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IObservableItemChecker<out TItem> where TItem : IFieldObject
    {
        /// <summary>
        /// アイテムを検出した
        /// </summary>
        IObservable<TItem> Checked { get; }
    }

    /// <summary>
    /// メダルの検出通知を受け取る（SerializeField用非ジェネリクス版）
    /// </summary>
    public interface IObservableMedalChecker : IObservableItemChecker<IMedal> { }
    /// <summary>
    /// アイテムの検出通知を受け取る（SerializeField用非ジェネリクス版）
    /// </summary>
    public interface IObservableFieldItemChecker : IObservableItemChecker<IFieldItem> { }
}