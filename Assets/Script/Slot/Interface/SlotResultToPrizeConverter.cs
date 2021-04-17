using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using Sirenix.OdinInspector;
using MedalPusher.Item.Checker;
using Zenject;
using MedalPusher.Slot.Internal;
using MedalPusher.Item.Payout;
using MedalPusher.Item;

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
        [Inject]
        private IFieldItemPayoutOperation fieldItemPayoutOperation;

        /// <summary>
        /// 当たったときに払い出すメダル数
        /// </summary>
        private static readonly int winMedals = 30;

        private void Start()
        {
            //当たったら30枚の払い出し要求
            resultSubmitter.ObservableSlotResult
                           .Where(result => result.IsWin)
                           .Subscribe(_ => medalPayoutOperation.PayoutRequest(winMedals));
            //7で当たったらJPChanceボールを払い出す
            resultSubmitter.ObservableSlotResult
                           .Where(result => result.WinRole == RoleValue.Role7)
                           .Subscribe(_ => fieldItemPayoutOperation.PayoutRequest<JPBall>());
        }
    }
}