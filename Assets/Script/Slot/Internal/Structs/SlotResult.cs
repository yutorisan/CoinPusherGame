using System;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの最終的な結果
    /// </summary>
    public readonly struct SlotResult
    {
        private readonly RoleSet roleSet;
        public SlotResult(RoleSet finalRoleSet)
        {
            this.roleSet = finalRoleSet;
        }

        /// <summary>
        /// 当たっているかどうか
        /// </summary>
        public bool IsWin => roleSet.IsBingo;
    }
}