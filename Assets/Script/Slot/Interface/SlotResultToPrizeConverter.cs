using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using Sirenix.OdinInspector;
using MedalPusher.Item.Checker;
using Zenject;
using MedalPusher.Slot.Internal;
using MedalPusher.Item.Payout;

namespace MedalPusher.Slot.Interface
{
    /// <summary>
    /// スロットの結果を景品に変換する
    /// </summary>
    public class SlotResultToPrizeConverter : MonoBehaviour
    {
        [Inject]
        private ISlotResultSubmitter resultSubmitter;
        [Inject]
        private IMedalPayoutOperation medalPayoutOperation;

        private void Start()
        {
            //当たったら30枚の払い出し要求
            resultSubmitter.ObservableSlotResult
                           .Where(result => result.IsWin)
                           .Subscribe(_ => medalPayoutOperation.PayoutRequest(30));
        }
    }
}