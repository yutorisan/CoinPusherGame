using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item;
using UnityEngine;

namespace MedalPusher.Lottery
{
    [Serializable]
    public struct LotteryPrizeInfo
    {
        [SerializeField]
        private int m_prizeMedals;

        public int PrizeMedals => m_prizeMedals;
    }
}