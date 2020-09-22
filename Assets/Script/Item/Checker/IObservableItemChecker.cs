using System;

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
        IObservable<TItem> ItemChecked { get; }
    }
}