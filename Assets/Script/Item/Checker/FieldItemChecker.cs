using UnityEngine;
using System.Collections;
using System;

namespace MedalPusher.Item.Checker
{
    public class FieldItemChecker : CheckerBase<IFieldItem>, IObservableFieldItemChecker
    {
        protected override string DetectTag => "FieldItem";
    }
}