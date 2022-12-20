using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Slot.Internal.Stock
{
    [Serializable]
    public struct StockLevelInfo
    {
        public int level;
        public int dividend;
        public Sprite stockImage;
    }
}