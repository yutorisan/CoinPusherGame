using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Item
{
    /// <summary>
    /// ジャックポッドチャンスボール アイテム
    /// </summary>
    public class JPBall : FieldItem
    {
        protected override IGameEvent GameEvent => new PayoutJPBallEvent();
    }
}
