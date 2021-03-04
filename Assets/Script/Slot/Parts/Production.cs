using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly Scenario _scenario;

        /// <summary>
        /// 通常のスロット演出を新規作成
        /// </summary>
        /// <param name="set">スロットの出目</param>
        public Production(Scenario scenario,
                          NormalRollProductionProperty normalProp,
                          ReachProductionProperty reachProp)
        {
            this._scenario = scenario;

            this.NormalProperty = normalProp;
            this.ReachProperty = reachProp;

            this.Left = new ReelProduction(scenario.Left);
            this.Middle = new ReelProduction(scenario.Middle);
            this.Right = new ReelProduction(scenario.Right);
        }

        public ReelProduction this[ReelPos index]
        {
            get
            {
                switch (index)
                {
                    case ReelPos.Left: return Left;
                    case ReelPos.Middle: return Middle;
                    case ReelPos.Right: return Right;
                    default: throw new InvalidEnumArgumentException();
                }
            }
        }

        public ReelProduction Left { get; }
        public ReelProduction Middle { get; }
        public ReelProduction Right { get; }

        public RoleSet FirstRoleset => _scenario.First;
        public RoleSet FinallyRoleset => _scenario.Finally;

        public bool IsReachProduction => _scenario.IsReachScenario;
        public bool IsWinProduction => _scenario.IsWinScenario;

        public NormalRollProductionProperty NormalProperty { get; }
        public ReachProductionProperty ReachProperty { get; }

        //public Scenario Scenario { get; }

   }

    /// <summary>
    /// 各Reelにおける演出
    /// </summary>
    public readonly struct ReelProduction
    {
        private readonly ReelScenario _scenario;
        /// <summary>
        /// 通常のリールの演出を新規作成。
        /// </summary>
        /// <param name="scenario"></param>
        public ReelProduction(ReelScenario scenario)
        {
            this._scenario = scenario;
        }

        public RoleValue FirstRole => _scenario.First;
        public RoleValue FinallyRole => _scenario.Finally;

        public bool IsReachProduction => _scenario.IsReachKey;

    }



}