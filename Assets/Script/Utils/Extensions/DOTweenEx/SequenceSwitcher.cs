using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using DG.Tweening;
using System.Linq;
using Sirenix.Utilities;
using UnityUtility.Linq;

namespace MedalPusher.Utils
{
    /// <summary>
    /// 登録した複数の<see cref="Sequence"/>の再生を切り替える
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class SequenceSwitcher<TKey> where TKey : struct
    {
        /// <summary>
        /// 登録されている<see cref="Sequence"/>
        /// </summary>
        private readonly Dictionary<TKey, Sequence> m_sequenceTable;

        public SequenceSwitcher()
        {
            m_sequenceTable = new Dictionary<TKey, Sequence>();
        }
        public SequenceSwitcher(Dictionary<TKey, Sequence> table)
        {
            m_sequenceTable = table;
        }

        /// <summary>
        /// <see cref="Sequence"/>を登録する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sequence"></param>
        public void Register(TKey key, Sequence sequence) =>
            m_sequenceTable.Add(key, sequence);

        /// <summary>
        /// <see cref="Sequence"/>の再生を切り替える
        /// </summary>
        /// <param name="key">切り替えるキー</param>
        public void SwitchTo(TKey key)
        {
            //再生中のSequenceを停止する
            if (PlayingKey.HasValue) m_sequenceTable[PlayingKey.Value].Pause();

            //SequenceTableにキーが含まれていたら再生する
            if (m_sequenceTable.ContainsKey(key))
            {
                m_sequenceTable[key].Play();
                PlayingKey = key;
            }
            else
            {
                PlayingKey = null;
            }
        }

        /// <summary>
        /// 再生中のキー
        /// </summary>
        public TKey? PlayingKey { get; private set; } = null;
    }
}