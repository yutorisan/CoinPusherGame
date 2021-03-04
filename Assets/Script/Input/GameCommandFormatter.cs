using System;
using System.Collections.Generic;
using MedalPusher.Input;
using UnityEngine;
using Zenject;
using UniRx;

namespace MedalPusher.Input
{
    public class GameCommandFormatter : IGameCommandProvider
    {
        #region singleton
        private static GameCommandFormatter m_instance;
        public static IGameCommandProvider Instance => m_instance ?? (m_instance = new GameCommandFormatter());

        private GameCommandFormatter()
        {
            ObservableGameCommand = m_userInput.ObservableInput
                                               .Where(key => keyCommandTable.ContainsKey(key))
                                               .Select(key => keyCommandTable[key]);
        }
        #endregion

        private IInputProvider m_userInput = new UserInputProvider();

        /// <summary>
        /// 入力とコマンドの紐付け
        /// キーコンフィグ
        /// </summary>
        private readonly Dictionary<KeyCode, GameCommand> keyCommandTable = new Dictionary<KeyCode, GameCommand>()
        {
            {KeyCode.Space, GameCommand.InputInspectorMedal},

            {KeyCode.A,  GameCommand.debug_Input500Medals}
        };

        public IObservable<GameCommand> ObservableGameCommand { get; }

    }
}