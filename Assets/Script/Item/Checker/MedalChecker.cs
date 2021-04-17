using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    public class MedalChecker : CheckerBase<IMedal>, IObservableMedalChecker
    {
        protected override string DetectTag => "Medal";
    }
}