using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace MedalPusher.Slot
{
    interface ISlotImpl
    {
        IObservable<string> ObservableSlotString { get; }
    }

    class SlotImpl
    {

    }

}
