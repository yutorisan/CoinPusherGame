using System;
using UnityEngine.EventSystems;
using UniRx.Triggers;
using UnityEngine;
using UniRx;

namespace MedalPusher.Input
{
    /// <summary>
    /// 指令されたGameCommandを提供する
    /// </summary>
    public interface IGameCommandProvider
    {
        /// <summary>
        /// 発行されたGameCommandの購読権
        /// </summary>
        IObservable<GameCommand> ObservableGameCommand { get; }
    }
    /// <summary>
    /// ユーザーからのキー入力を監視して
    /// キーコンフィグに従ってGameCommandを発行する
    /// </summary>
    internal class GameCommandProvider : IGameCommandProvider
    {
        /// <summary>
        /// 入力されたKeyCodeを発行するIObservable
        /// </summary>
        private readonly IObservable<KeyCode> observableKeyCode = Observable.Empty<KeyCode>();
        /// <summary>
        /// 使用するキーコンフィグ
        /// </summary>
        private readonly IKeyConfig keyConfig = KeyConfigSelector.Now;

        public GameCommandProvider()
        {
            //使用中のKeyCodeを取得して、入力監視対象に追加する
            foreach (var key in keyConfig.UsedKeyCodes)
            {
                observableKeyCode = observableKeyCode.Merge(KeyInputed(key));
            }
        }


        public IObservable<GameCommand> ObservableGameCommand =>
            observableKeyCode.Select(key => keyConfig.KeyCommandTable[key]) //KeyCodeをGameCommandに変換する
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