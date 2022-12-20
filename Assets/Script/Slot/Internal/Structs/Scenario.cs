using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        private Scenario(RoleSet roleset)
        {
            this.First = roleset;
            this.Finally = roleset;

            this.Left = ReelScenario.Normal(roleset.Left);
            this.Middle = ReelScenario.Normal(roleset.Middle);
            this.Right = ReelScenario.Normal(roleset.Right);
        }
        /// <summary>
        /// リーチのシナリオを新規作成
        /// </summary>
        /// <param name="reachedRoleSet">リーチになっているセット</param>
        /// <param name="finally">リーチ再抽選後にキーとなるリールが示すRole</param>
        private Scenario(RoleSet reachedRoleSet, RoleValue @finally)
        {
            if (!reachedRoleSet.IsReach) throw new InvalidOperationException("入力されたセットはリーチではありません");

            this.First = reachedRoleSet;
            switch (First.ReachStatus.Value.ReachReelPos)
            {
                case ReelPos.Left:
                    Left = ReelScenario.ReachKey(reachedRoleSet.Left, @finally);
                    Middle = ReelScenario.Normal(reachedRoleSet.Middle);
                    Right = ReelScenario.Normal(reachedRoleSet.Right);
                    Finally = new RoleSet(@finally, reachedRoleSet.Middle, reachedRoleSet.Right);
                    break;
                case ReelPos.Middle:
                    Left = ReelScenario.Normal(reachedRoleSet.Left);
                    Middle = ReelScenario.ReachKey(reachedRoleSet.Middle, @finally);
                    Right = ReelScenario.Normal(reachedRoleSet.Right);
                    Finally = new RoleSet(reachedRoleSet.Left, @finally, reachedRoleSet.Right);
                    break;
                case ReelPos.Right:
                    Left = ReelScenario.Normal(reachedRoleSet.Left);
                    Middle = ReelScenario.Normal(reachedRoleSet.Middle);
                    Right = ReelScenario.ReachKey(reachedRoleSet.Right, @finally);
                    Finally = new RoleSet(reachedRoleSet.Left, reachedRoleSet.Middle, @finally);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
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

        public ReelScenario this[ReelPos index]
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

        public ReelScenario Left { get; }
        public ReelScenario Middle { get; }
        public ReelScenario Right { get; }

        /// <summary>
        /// 最初に表示する出目
        /// </summary>
        public RoleSet First { get; }
        /// <summary>
        /// 最終的に表示する出目
        /// </summary>
        public RoleSet Finally { get; }

        public RoleSet ResultRoleSet =>
            new RoleSet(Left.Finally, Middle.Finally, Right.Finally);
        public bool IsReachScenario => First.IsReach;
        public bool IsWinScenario => Finally.IsBingo;
    }

    /// <summary>
    /// 各リールのシナリオ
    /// </summary>
    public readonly struct ReelScenario
    {
        private ReelScenario(RoleValue first, RoleValue @finally, bool isReachKey)
        {
            this.First = first;
            this.Finally = @finally;
            this.IsReachKey = isReachKey;
        }

        /// <summary>
        /// 1回の抽選で終わる通常のシナリオを作成
        /// </summary>
        /// <param name="toValue">表示する出目</param>
        /// <returns></returns>
        public static ReelScenario Normal(RoleValue toValue)
        {
            return new ReelScenario(toValue, toValue, false);
        }
        public static ReelScenario ReachKey(RoleValue first, RoleValue @finally)
        {
            return new ReelScenario(first, @finally, true);
        }

        /// <summary>
        /// 最初に表示する出目
        /// </summary>
        public RoleValue First { get; }
        /// <summary>
        /// 最終的に表示する出目
        /// </summary>
        public RoleValue Finally { get; }
        /// <summary>
        /// このリールシナリオはリーチのキーシナリオかどうか
        /// </summary>
        public bool IsReachKey { get; }
    }
}