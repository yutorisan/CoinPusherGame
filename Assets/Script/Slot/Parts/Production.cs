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



}