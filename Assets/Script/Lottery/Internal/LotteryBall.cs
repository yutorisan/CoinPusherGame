using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item;
using UnityEngine;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台で使用する抽選ボール
    /// </summary>
    public class LotteryBall : MonoBehaviour, IFieldObject
    {
        public void Dispose() => Destroy(gameObject);
    }
}