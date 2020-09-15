using System;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// メダル払出し要求する
    /// </summary>
    public interface IMedalPayoutOperator
    {
        /// <summary>
        /// メダル払出を要求する
        /// </summary>
        /// <param name="medals">払出しメダル数</param>
        void Payout(int medals);
    }
}