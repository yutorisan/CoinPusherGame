using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Slot
{
    public enum SlotProductionStatus
    {
        /// <summary>
        /// アイドル状態
        /// </summary>
        Idol,
        /// <summary>
        /// 通常の回転シーケンス中
        /// </summary>
        Rolling,
        /// <summary>
        /// リーチ演出中
        /// </summary>
        Reaching,
        /// <summary>
        /// 当たり演出中
        /// </summary>
        Winning,
    }
}