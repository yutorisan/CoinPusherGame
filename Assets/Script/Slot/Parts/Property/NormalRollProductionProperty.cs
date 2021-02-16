using System;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot
{
    [Serializable, LabelText("通常回転シーケンス")]
    public struct NormalRollProductionProperty
    {
        /// <su mmary>
        /// 周回数
        /// </summary>
        [InfoBox("周回数", InfoMessageType.None)]
        public int Laps;
        /// <summary>
        /// 最大回転速度
        /// </summary>
        [InfoBox("最大回転速度", InfoMessageType.None)]
        public float MaxRps;
        /// <summary>
        /// 加速時間
        /// </summary>
        [InfoBox("加速時間", InfoMessageType.None)]
        public float AccelDuration;
        /// <summary>
        /// 減速時間
        /// </summary>
        [InfoBox("減速時間", InfoMessageType.None)]
        public float DeceleDuration;
    }
}