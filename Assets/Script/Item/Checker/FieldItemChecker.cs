using UnityEngine;
using System.Collections;
using System;

namespace MedalPusher.Item.Checker
{
    public class FieldItemChecker : MonoBehaviour, IObservableItemChecker<IFieldItem>
    {
        public IObservable<IFieldItem> Checked => throw new NotImplementedException();

        public IObservable<int> Count => throw new NotImplementedException();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}