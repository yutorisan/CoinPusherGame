using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    ///// <summary>
    ///// 抽選の景品を投入する口。
    ///// ここに放り込んでおけば適当なタイミングでフィールドに渡してくれる
    ///// </summary>
    //public interface ILotteryPrizeInsertionSlot
    //{
    //    /// <summary>
    //    /// 手に入れた景品を投入する
    //    /// </summary>
    //    /// <param name="prizeInfo"></param>
    //    void InsertPrize(LotteryPrizeInfo prizeInfo);
    //}
    //public class LotteryPrizeCollector : MonoBehaviour, ILotteryPrizeInsertionSlot
    //{
    //    [Inject]
    //    private IMedalPayoutOperation _medalPayouter;
    //    //[Inject]
    //    //private IFieldItemPayoutOperator _fieldItemPayouter;

    //    // Start is called before the first frame update
    //    void Start()
    //    {

    //    }

    //    public void InsertPrize(LotteryPrizeInfo prizeInfo)
    //    {
    //        //メダル払出し要求
    //        _medalPayouter.PayoutRequest(prizeInfo.PrizeMedals);
    //    }
    //}
}