using System;
using MedalPusher.Slot.Internal.Stock;

namespace MedalPusher.Slot
{
    public interface IReadOnlySlotResult
    {
        bool IsWin { get; }
        RoleValue? WinRole { get; }
        StockLevelInfo StockLevelInfo { get; }
    }
    public interface ISlotResult : IReadOnlySlotResult
    {
        void SetStockLevelInfo(StockLevelInfo info);
    }
    /// <summary>
    /// スロットの最終的な結果
    /// </summary>
    public class SlotResult : ISlotResult
    {
        private readonly RoleSet roleSet;
        public SlotResult(RoleSet finalRoleSet)
        {
            this.roleSet = finalRoleSet;
        }

        public void SetStockLevelInfo(StockLevelInfo info)
        {
            this.StockLevelInfo = info;
        }

        public StockLevelInfo StockLevelInfo { get; private set; }

        /// <summary>
        /// 当たっているかどうか
        /// </summary>
        public bool IsWin => roleSet.IsBingo;
        /// <summary>
        /// 当たっている場合、当たった役
        /// </summary>
        public RoleValue? WinRole
        {
            get
            {
                if (IsWin) return roleSet.Left;
                else return null;
            }
        }
    }
}