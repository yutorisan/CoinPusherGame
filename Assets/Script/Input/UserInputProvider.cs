using System;
using MedalPusher.Utils;
using UniRx;
using UnityEngine;

namespace MedalPusher.Input
{
    /// <summary>
    /// ユーザーからのキー入力を提供する
    /// </summary>
    internal class UserInputProvider : IInputProvider
    {
        public UserInputProvider()
        {
            //KeyConfigに使用されているキーをすべて監視する
            IObservable<KeyCode> observableKeyCode = Observable.Empty<KeyCode>();
            foreach (var code in KeyConfigProvider.Now.KeyCommandTable.Keys)
            {
                observableKeyCode = observableKeyCode.Merge(ObservableEx.FromKeyCode(code));
            }
            this.ObservableKeyCode = observableKeyCode.Share();
        }

        public IObservable<KeyCode> ObservableKeyCode { get; }
    }
}