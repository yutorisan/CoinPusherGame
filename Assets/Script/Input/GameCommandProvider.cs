using System;
using UnityEngine.EventSystems;
using UniRx.Triggers;
using UnityEngine;
using UniRx;
using Zenject;

namespace MedalPusher.Input
{
    /// <summary>
    /// 指令されたGameCommandを提供する
    /// </summary>
    public interface IGameCommandProvider
    {
        /// <summary>
        /// 発行されたGameCommand
        /// </summary>
        IObservable<GameCommand> ObservableGameCommand { get; }
    }
    /// <summary>
    /// ユーザーからのキー入力を監視して
    /// キーコンフィグに従ってGameCommandを発行する
    /// </summary>
    internal class GameCommandProvider : IGameCommandProvider, IInitializable
    {
        [Inject]
        private IInputProvider input;

        //IInputProviderが注入されてから初期化を行うため
        //コンストラクタではなくInitializeを使用
        public void Initialize()
        {
            ObservableGameCommand =
                input.ObservableKeyCode
                     .Select(key => KeyConfigProvider.Now.KeyCommandTable[key]) //KeyCodeをKeyConfigによってGameCommandに変換する
                     .Share();
        }

        public IObservable<GameCommand> ObservableGameCommand { get; private set; }


    }

    /// <summary>
    /// 入力情報を提供する
    /// </summary>
    internal interface IInputProvider
    {
        IObservable<KeyCode> ObservableKeyCode { get; }
    }
}