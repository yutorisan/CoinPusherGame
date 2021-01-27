using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot.Prize
{
    /// <summary>
    /// スロットの景品をオーダーする
    /// </summary>
    public interface ISlotPrizeOrderer
    {
        void Order(SlotResult result);
    }
    public class SlotPrizeOrderer : ISlotPrizeOrderer
    {
        [Inject]
        private IMedalPayoutOperation m_medalPayoutOperation;

        public void Order(SlotResult result)
        {
            if (result.RoleSet.IsBingo)
            {
                //あたったら一律20枚払い出し
                m_medalPayoutOperation.PayoutRequest(20);
            }
        }
    }
}