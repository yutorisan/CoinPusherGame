using System.Collections;
using System.Collections.Generic;
using MedalPusher.Lottery;
using UnityEngine;

namespace MedalPusher.Item
{
    /// <summary>
    /// ジャックポッドチャンスボール アイテム
    /// </summary>
    public class JPBall : FieldItem
    {
        private IBallBornOperator ballBornOperator;

        private void Start()
        {
            ballBornOperator = GameObject.Find("BallBorner").GetComponent<IBallBornOperator>();
        }

        protected override IGameEvent GameEvent => new PayoutJPBallEvent(ballBornOperator);
    }
}
