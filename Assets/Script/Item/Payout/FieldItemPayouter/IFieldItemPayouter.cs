using System;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// アイテムを払い出すことができる
    /// </summary>
    public interface IFieldItemPayouter
    {
        /// <summary>
        /// アイテムを払い出す
        /// </summary>
        /// <param name="item">払い出すアイテムオブジェクトインスタンス</param>
        void Payout(FieldItem item);
    }
}