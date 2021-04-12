using UnityEngine;
using System.Collections;
using System;

namespace MedalPusher.Item.Checker
{
    public class FieldItemChecker : MonoBehaviour, IObservableItemChecker<IFieldItem>
    {
        public IObservable<IFieldItem> Checked => throw new NotImplementedException();

        public IObservable<int> Count => throw new NotImplementedException();

        void Start()
        {
            throw new NotImplementedException();
        }
    }
}