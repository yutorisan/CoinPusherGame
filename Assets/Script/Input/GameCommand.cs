using System;

namespace MedalPusher.Input
{
    public enum GameCommand
    {
        /// <summary>
        /// インスペクタで所持しているメダルをフィールドに投入する
        /// </summary>
        InputInspectorMedal,

        /// <summary>
        /// デバッグ用_500枚のメダルをフィールドに払い出す
        /// </summary>
        debug_Input500Medals,
    }
}