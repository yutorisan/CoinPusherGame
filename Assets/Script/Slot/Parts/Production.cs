using System;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出
    /// </summary>
    public struct Production
    {
        public Production(RoleSet set)
        {
            this.RoleSet = set;

            LeftPart = new ProductionPart(set.Left);
            MiddlePart = new ProductionPart(set.Middle);
            RightPart = new ProductionPart(set.Right);
        }

        public RoleSet RoleSet { get; }

        public ProductionPart LeftPart { get; }
        public ProductionPart MiddlePart { get; }
        public ProductionPart RightPart { get; }
    }

    /// <summary>
    /// 各Roleにおける演出
    /// </summary>
    public struct ProductionPart
    {
        public ProductionPart(RoleValue roleValue)
        {
            DisplayRole = roleValue;
        }
        /// <summary>
        /// この演出における出目
        /// </summary>
        public RoleValue DisplayRole { get; }
    }
}