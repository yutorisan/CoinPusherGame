using System;
using System.Collections.Generic;
using MedalPusher.Input;
using UnityEngine;
using Zenject;

namespace MedalPusher.Input
{
    public class GameCommandFormatter
    {
        private IInputProvider m_userInput = new UserInputProvider();

        /// <summary>
        /// 入力とコマンドの紐付け
        /// キーコンフィグ
        /// </summary>
        private readonly Dictionary<KeyCode, GameCommand> keyCommandTable = new Dictionary<KeyCode, GameCommand>()
        {
            {KeyCode.Space, GameCommand.InputInspectorMedal}
        };

        /// <summary>
        /// fff
        /// </summary>
        public GameCommandFormatter()
        {
            //m_userInput.ObservableInput.Subscribe
        }
    }
}