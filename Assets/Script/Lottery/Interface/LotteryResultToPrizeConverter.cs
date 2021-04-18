using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Payout;
using MedalPusher.Lottery;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher
{
    /// <summary>
    /// 抽選台の抽選結果を景品に変換する
    /// </summary>
    public class LotteryResultToPrizeConverter : MonoBehaviour
    {
        [Inject]
        ILotteryResultSubmitter resultSubmitter;
        [Inject]
        IMedalPayoutOperation medalPayoutOperation;
        [Inject]
        IFieldItemPayoutOperation fieldItemPayoutOperation;

        void Start()
        {
            //景品を受け取ったらメダルを払い出す
            resultSubmitter.LotteryResult.Subscribe(info => medalPayoutOperation.PayoutRequest(info.PrizeMedals, MedalPayoutMethod.Shower));
        }
    }
}
