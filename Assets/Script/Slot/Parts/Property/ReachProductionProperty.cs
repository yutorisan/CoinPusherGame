using System;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot
{
    [Serializable, LabelText("リーチ演出シーケンス")]
    public struct ReachProductionProperty
    {
        /// <summary>
        /// 切り替わりシーケンスの長さ
        /// </summary>
        [InfoBox("切り替わりシーケンスの継続時間", InfoMessageType.None)]
        public float SwitchingDuraion;
        /// <summary>
        /// 高速切り替わりシーケンスの長さ
        /// </summary>
        [InfoBox("切り替わりシーケンスの継続時間（高速モード）", InfoMessageType.None)]
        public float FastSwitchingDuration;
        /// <summary>
        /// 切り替わりの間隔
        /// </summary>
        [InfoBox("切り替わりの間隔", InfoMessageType.None)]
        public float SwitchInterval;
        /// <summary>
        /// 高速切り替わりの間隔
        /// </summary>
        [InfoBox("切り替わりの間隔（高速モード）", InfoMessageType.None)]
        public float FastSwitchInterval;
        /// <summary>
        /// 高速モードのしきい値
        /// 高速モードにする最低切り替わり残数
        /// </summary>
        [InfoBox("高速モードにする最低切り替わり残数", InfoMessageType.None)]
        public int FastModeThreshold;

        public ReachAntagonismProduction AntagonismType { get; set; }
    }

    public enum ReachAntagonismProduction
    {
        Direct,
        Antagonism
    }
}