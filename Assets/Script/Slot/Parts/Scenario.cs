using System;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの抽選シナリオ
    /// </summary>
    public readonly struct Scenario
    {
        /// <summary>
        /// ノーマルのシナリオを新規作成
        /// </summary>
        /// <param name="roleset"></param>
        public Scenario(RoleSet roleset)
        {
            this.FirstRoleset = roleset;

            LeftScenario = new ReelScenario(roleset.Left);
            MiddleScenario = new ReelScenario(roleset.Middle);
            RightScenario = new ReelScenario(roleset.Right);
        }
        /// <summary>
        /// リーチのシナリオを新規作成
        /// </summary>
        /// <param name="reachedRoleSet">リーチになっているセット</param>
        /// <param name="afterReachRole">リーチ再抽選後にキーとなるリールが示すRole</param>
        public Scenario(RoleSet reachedRoleSet, RoleValue afterReachRole)
        {
            if (!reachedRoleSet.IsReach) throw new InvalidOperationException("入力されたセットはリーチではありません");

            this.FirstRoleset = reachedRoleSet;
            switch (FirstRoleset.ReachStatus.Value.ReachReelPos)
            {
                case ReelPos.Left:
                    LeftScenario = new ReelScenario(reachedRoleSet.Left, afterReachRole);
                    MiddleScenario = new ReelScenario(reachedRoleSet.Middle);
                    RightScenario = new ReelScenario(reachedRoleSet.Right);
                    break;
                case ReelPos.Middle:
                    LeftScenario = new ReelScenario(reachedRoleSet.Left);
                    MiddleScenario = new ReelScenario(reachedRoleSet.Middle, afterReachRole);
                    RightScenario = new ReelScenario(reachedRoleSet.Right);
                    break;
                case ReelPos.Right:
                    LeftScenario = new ReelScenario(reachedRoleSet.Left);
                    MiddleScenario = new ReelScenario(reachedRoleSet.Middle);
                    RightScenario = new ReelScenario(reachedRoleSet.Right, afterReachRole);
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }
        /// <summary>
        /// 最初に表示する出目
        /// </summary>
        public RoleSet FirstRoleset { get; }

        public ReelScenario LeftScenario { get; }
        public ReelScenario MiddleScenario { get; }
        public ReelScenario RightScenario { get; }

        public SlotResult Result =>
            new SlotResult(new RoleSet(LeftScenario.FinallyRole, MiddleScenario.FinallyRole, RightScenario.FinallyRole));
    }

    /// <summary>
    /// 各リールのシナリオ
    /// </summary>
    public readonly struct ReelScenario
    {
        /// <summary>
        /// 通常のシナリオを作成
        /// </summary>
        /// <param name="role"></param>
        public ReelScenario(RoleValue role)
        {
            this.FirstRoleValue = role;
            this.AfterReachRole = null;
        }
        /// <summary>
        /// リーチのキーとなるシナリオを作成
        /// </summary>
        /// <param name="first">最初に表示するRole</param>
        /// <param name="afterReachRole">リーチの再抽選の先に表示するRole</param>
        public ReelScenario(RoleValue first, RoleValue afterReachRole)
        {
            this.FirstRoleValue = first;
            this.AfterReachRole = afterReachRole;
        }

        /// <summary>
        /// 最初に表示する出目
        /// </summary>
        public RoleValue FirstRoleValue { get; }
        /// <summary>
        /// リーチ演出の先に表示する確定出目
        /// </summary>
        public RoleValue? AfterReachRole { get; }

        /// <summary>
        /// このリールシナリオはリーチのキーシナリオかどうか
        /// </summary>
        public bool IsReachReelScenario => AfterReachRole.HasValue;
        /// <summary>
        /// 最終的に表示する出目
        /// </summary>
        public RoleValue FinallyRole => AfterReachRole ?? FirstRoleValue;
    }
}