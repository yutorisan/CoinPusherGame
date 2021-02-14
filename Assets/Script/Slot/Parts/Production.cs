using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.UIElements;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出
    /// </summary>
    public readonly struct Production
    {
        private readonly IReadOnlyDictionary<ReelPos, ReelProduction> m_reelProductionTable;

        /// <summary>
        /// 通常のスロット演出を新規作成
        /// </summary>
        /// <param name="set">スロットの出目</param>
        public Production(Scenario scenario,
                          NormalRollProductionProperty normalProp,
                          ReachProductionProperty reachProp)
        {
            this.Scenario = scenario;
            this.NormalProperty = normalProp;
            this.ReachProperty = reachProp;

            m_reelProductionTable = new Dictionary<ReelPos, ReelProduction>()
            {
                {ReelPos.Left, new ReelProduction(scenario.GetReelScenario(ReelPos.Left)) },
                {ReelPos.Middle, new ReelProduction(scenario.GetReelScenario(ReelPos.Middle)) },
                {ReelPos.Right, new ReelProduction(scenario.GetReelScenario(ReelPos.Right)) }
            };
        }

        public ReelProduction GetReelProduction(ReelPos pos) => m_reelProductionTable[pos];

        public NormalRollProductionProperty NormalProperty { get; }
        public ReachProductionProperty ReachProperty { get; }

        public Scenario Scenario { get; }

   }

    /// <summary>
    /// 各Reelにおける演出
    /// </summary>
    public readonly struct ReelProduction
    {
        /// <summary>
        /// 通常のリールの演出を新規作成。
        /// </summary>
        /// <param name="scenario"></param>
        public ReelProduction(ReelScenario scenario)
        {
            this.ReelScenario = scenario;
        }

        public ReelScenario ReelScenario { get; }
    }

    [Serializable, LabelText("通常回転シーケンス")]
    public struct NormalRollProductionProperty
    {
        /// <summary>
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
    }
}