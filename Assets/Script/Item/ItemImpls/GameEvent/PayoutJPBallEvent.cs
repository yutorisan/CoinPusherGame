using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item;
using UnityEngine;
using Zenject;
using MedalPusher.Lottery;

namespace MedalPusher
{
    /// <summary>
    /// JPBallを払い出すイベント
    /// </summary>
    public class PayoutJPBallEvent : GameEvent
    {
        private readonly IBallBornOperator ballBornOperator;

        public PayoutJPBallEvent(IBallBornOperator ballBornOperator)
        {
            this.ballBornOperator = ballBornOperator;
        }

        public override void Occur()
        {
            ballBornOperator.BornRequest();
        }
    }
}
