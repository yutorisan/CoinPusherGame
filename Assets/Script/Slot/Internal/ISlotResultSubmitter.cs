using System;

namespace MedalPusher.Slot.Internal
{
    /// <summary>
    /// スロットの回転結果の通知を発行する
    /// </summary>
    public interface ISlotResultSubmitter
    {
        IObservable<SlotResult> ObservableSlotResult { get; }
    }
}