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
        internal static IKeyConfig Default => DefalutKeyConfig.Instance;

        /// <summary>
        /// デフォルトのキーコンフィグ
        /// </summary>
        private class DefalutKeyConfig : KeyConfig
        {
            private static IKeyConfig _instance;
            public static IKeyConfig Instance => _instance ?? (_instance = new DefalutKeyConfig());

            private static readonly Dictionary<KeyCode, GameCommand> m_keyCommandTable = new Dictionary<KeyCode, GameCommand>()
            {
                {KeyCode.Space, GameCommand.InputInspectorMedal},
                {KeyCode.A,  GameCommand.debug_Input500Medals}
            };

            public override IReadOnlyDictionary<KeyCode, GameCommand> KeyCommandTable => m_keyCommandTable;
        }
    }
}