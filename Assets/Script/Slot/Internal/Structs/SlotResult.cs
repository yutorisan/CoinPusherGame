using System;

namespace MedalPusher.Slot
{
    public struct SlotResult
    {
        public SlotResult(RoleSet result)
        {
            this.RoleSet = result;
        }
        public RoleSet RoleSet { get; }
    }
}