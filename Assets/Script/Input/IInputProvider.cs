using System;
using UnityEngine;

namespace MedalPusher.Input
{
    /// <summary>
    /// ユーザー入力を統括して発信する
    /// </summary>
    public interface IInputProvider
    {
        IObservable<UnityEngine.KeyCode> ObservableInput { get; }
    }
}