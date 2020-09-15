using System;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// メダルの払出し状況の購読を提供する
    /// </summary>
    public interface IObservableMedalPayouter
    {
        /// <summary>
        /// 払出し待機中のメダル数の購読を提供
        /// </summary>
        IObservable<int> PayoutStockMedals { get; }
    }
}