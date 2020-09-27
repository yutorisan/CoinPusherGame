using System;
using UnityEngine.EventSystems;
using UniRx.Triggers;
using UnityEngine;
using UniRx;

namespace MedalPusher.Input
{
    public class UserInputProvider : IInputProvider
    {
        public UserInputProvider()
        {
            //InputObservable.FromKeyCode(KeyCode.Space)
                           
        }

        public IObservable<KeyCode> ObservableInput => InputObservable.FromAnyKey().Share();
    }
}