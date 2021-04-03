using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;

namespace MedalPusher.Input
{
    /// <summary>
    /// 使用するキーコンフィグを選択する
    /// </summary>
    internal static class KeyConfigSelector 
    {
        /// <summary>
        /// 現在のキーコンフィグ設定を取得する
        /// </summary>
        internal static IKeyConfig Now => DefalutKeyConfig.Instance;

        /// <summary>
        /// デフォルトのキーコンフィグ
        /// </summary>
        private class DefalutKeyConfig : KeyConfig
        {
            //シングルトン
            private static IKeyConfig _instance;
            public static IKeyConfig Instance => _instance ?? (_instance = new DefalutKeyConfig());

            /// <summary>
            /// キーコンフィグ本体
            /// </summary>
            private static readonly Dictionary<KeyCode, GameCommand> m_keyCommandTable = new Dictionary<KeyCode, GameCommand>()
            {
                {KeyCode.Space, GameCommand.InputInspectorMedal},
                {KeyCode.A,  GameCommand.debug_Input500Medals}
            };

            public override IReadOnlyDictionary<KeyCode, GameCommand> KeyCommandTable => m_keyCommandTable;
        }
    }
}