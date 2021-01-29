using System;
using System.Runtime.CompilerServices;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの抽選シナリオ
    /// </summary>
    public readonly struct Scenario
    {
        private readonly ReelScenario m_leftScenario, m_milldeScenario, m_rightScenario;

        /// <summary>
        /// ノーマルのシナリオを新規作成
        /// </summary>
        /// <param name="roleset"></param>
        private Scenario(RoleSet roleset)
        {
            this.FirstRoleset = roleset;
            this.FinalRoleset = roleset;

            m_leftScenario = new ReelScenario(roleset[ReelPos.Left]);
            m_milldeScenario = new ReelScenario(roleset[ReelPos.Middle]);
            m_rightScenario = new ReelScenario(roleset[ReelPos.Right]);
        }
        /// <summary>
        /// リーチのシナリオを新規作成
        /// </summary>
        /// <param name="reachedRoleSet">リーチになっているセット</param>
        /// <param name="afterReachRole">リーチ再抽選後にキーとなるリールが示すRole</param>
        private Scenario(RoleSet reachedRoleSet, RoleValue afterReachRole)
        {
            if (!reachedRoleSet.IsReach) throw new InvalidOperationException("入力されたセットはリーチではありません");

            this.FirstRoleset = reachedRoleSet;
            switch (FirstRoleset.ReachStatus.Value.ReachReelPos)
            {
                case ReelPos.Left:
                    m_leftScenario = new ReelScenario(reachedRoleSet[ReelPos.Left], afterReachRole);
                    m_milldeScenario = new ReelScenario(reachedRoleSet[ReelPos.Middle]);
                    m_rightScenario = new ReelScenario(reachedRoleSet[ReelPos.Right]);
                    FinalRoleset = new RoleSet(afterReachRole, reachedRoleSet[ReelPos.Middle], reachedRoleSet[ReelPos.Right]);
                    break;
                case ReelPos.Middle:
                    m_leftScenario = new ReelScenario(reachedRoleSet[ReelPos.Left]);
                    m_milldeScenario = new ReelScenario(reachedRoleSet[ReelPos.Middle], afterReachRole);
                    m_rightScenario = new ReelScenario(reachedRoleSet[ReelPos.Right]);
                    FinalRoleset = new RoleSet(reachedRoleSet[ReelPos.Left], afterReachRole, reachedRoleSet[ReelPos.Right]);
                    break;
                case ReelPos.Right:
                    m_leftScenario = new ReelScenario(reachedRoleSet[ReelPos.Left]);
                    m_milldeScenario = new ReelScenario(reachedRoleSet[ReelPos.Middle]);
                    m_rightScenario = new ReelScenario(reachedRoleSet[ReelPos.Right], afterReachRole);
                    FinalRoleset = new RoleSet(reachedRoleSet[ReelPos.Left], reachedRoleSet[ReelPos.Middle], afterReachRole);
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// ハズレのシナリオを作成
        /// </summary>
        /// <param name="roleSet"></param>
        /// <returns></returns>
        public static Scenario Lose(RoleSet roleSet)
        {
            return new Scenario(roleSet);
        }
        /// <summary>
        /// リーチのシナリオを作成
        /// </summary>
        /// <param name="reachedRoleSet">リーチのRoleset</param>
        /// <param name="afterReachRole">リーチ再抽選後にキーとなるリールが示すRole</param>
        /// <returns></returns>
        public static Scenario Reach(RoleSet reachedRoleSet, RoleValue afterReachRole)
        {
            return new Scenario(reachedRoleSet, afterReachRole);
        }
        /// <summary>
        /// ダイレクトで当たるシナリオを作成
        /// </summary>
        /// <param name="winRole"></param>
        /// <returns></returns>
        public static Scenario DirectWin(RoleValue winRole)
        {
            return new Scenario(new RoleSet(winRole, winRole, winRole));
        }

        public ReelScenario GetReelScenario(ReelPos pos)
        {
            switch (pos)
            {
                case ReelPos.Left: return m_leftScenario;
                case ReelPos.Middle: return m_milldeScenario;
                case ReelPos.Right: return m_rightScenario;
                default: throw new Exception();
            }
        }

        /// <summary>
        /// 最初に表示する出目
        /// </summary>
        public RoleSet FirstRoleset { get; }
        /// <summary>
        /// 最終的に表示する出目
        /// </summary>
        public RoleSet FinalRoleset { get; }

        public SlotResult Result =>
            new SlotResult(new RoleSet(m_leftScenario.FinallyRole, m_milldeScenario.FinallyRole, m_rightScenario.FinallyRole));
        public bool IsReachScenario => FirstRoleset.IsReach;
        public bool IsWinScenario => FinalRoleset.IsBingo;
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