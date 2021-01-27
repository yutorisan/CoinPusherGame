using System;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出
    /// </summary>
    public readonly struct Production
    {
        /// <summary>
        /// 通常のスロット演出を新規作成
        /// </summary>
        /// <param name="set">スロットの出目</param>
        public Production(Scenario scenario)
        {
            this.Scenario = scenario;

            this.LeftPart = new ReelProduction(scenario.LeftScenario);
            this.MiddlePart = new ReelProduction(scenario.MiddleScenario);
            this.RightPart = new ReelProduction(scenario.RightScenario);
        }

        public Scenario Scenario { get; }

        public ReelProduction LeftPart { get; }
        public ReelProduction MiddlePart { get; }
        public ReelProduction RightPart { get; }

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
            //this.FirstStopRole = scenario.FirstRoleValue;
            //this.ReachToWinRoleValue = null;
        }

        public ReelScenario ReelScenario { get; }

        ///// <summary>
        ///// このリールがリーチのキーとなる場合の演出を新規作成。
        ///// </summary>
        ///// <param name="scenario"></param>
        ///// <param name="reachToWinRoleValue"></param>
        //public ReelProduction(ReelScenario scenario, RoleValue reachToWinRoleValue)
        //{
        //    this.FirstStopRole = roleValue;
        //    this.ReachToWinRoleValue = reachToWinRoleValue;
        //}
        ///// <summary>
        ///// この演出における最初にストップする出目
        ///// </summary>
        //public RoleValue FirstStopRole { get; }
        ///// <summary>
        ///// リーチ演出時に、リールが最終的に表示するべき出目
        ///// </summary>
        //public RoleValue? ReachToWinRoleValue { get; }
        ///// <summary>
        ///// リーチ演出対象のリールかどうか
        ///// </summary>
        //public bool IsReachReel => ReachToWinRoleValue.HasValue;
    }
}