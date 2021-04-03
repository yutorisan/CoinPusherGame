using System;
using UnityEngine.EventSystems;
using UniRx.Triggers;
using UnityEngine;
using UniRx;

namespace MedalPusher.Input
{
    internal class UserInputProvider : IGameCommandProvider
    {
        private readonly IObservable<KeyCode> observableKeyCode = Observable.Empty<KeyCode>();
        private readonly IKeyConfig keyConfig = KeyConfigSelector.Default;

        public UserInputProvider()
        {
            //使用中のキーコードの入力を監視する
            foreach (var key in keyConfig.UsedKeyCodes)
            {
                observableKeyCode = observableKeyCode.Merge(KeyInputed(key));
            }
        }


        public IObservable<GameCommand> ObservableGameCommand =>
            observableKeyCode.Select(key => keyConfig.KeyCommandTable[key])
                             .Share();

        /// <summary>
        /// 特定のキーが押されたことを示すIObservableを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private IObservable<KeyCode> KeyInputed(KeyCode key) =>
            Observable.EveryUpdate()
                      .Where(_ => UnityEngine.Input.GetKey(key))
                      .Select(_ => key);
    }
}