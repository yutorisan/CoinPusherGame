using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item;
using UnityEngine;

namespace MedalPusher
{
    /// <summary>
    /// JPBallを払い出すイベント
    /// </summary>
    public class PayoutJPBallEvent : GameEvent
    {
        public override void Occur()
        {
            UnityEngine.Debug.Log("たまを獲得！");
        }
    }
}
