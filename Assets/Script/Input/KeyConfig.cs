using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;

namespace MedalPusher.Input
{
    internal interface IKeyConfig
    {
        /// <summary>
        /// キーコンフィグ本体
        /// </summary>
        IReadOnlyDictionary<KeyCode, GameCommand> KeyCommandTable { get; }
    }
    /// <summary>
    /// キーコンフィグ設定クラスのベースクラス
    /// </summary>
    internal abstract class KeyConfig : IKeyConfig
    {
        public abstract IReadOnlyDictionary<KeyCode, GameCommand> KeyCommandTable { get; }
    }
}