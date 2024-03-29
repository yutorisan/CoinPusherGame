﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Permissions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MedalPusher.Utils;
using UnityUtility.Linq;

namespace MedalPusher.Slot.Internal.Core
{
    /// <summary>
    /// ReelSequenceの実データ（Sequence）に対するアクセス
    /// </summary>
    public interface IReelSequenceData : IEnumerable<KeyValuePair<RoleValue, Sequence>>
    {
        Sequence this[RoleValue index] { get; set; }
    }
    /// <summary>
    /// Reelに対する制御シーケンス
    /// </summary>
    public interface IReelSequence : IReelSequenceData
    {
        /// <summary>
        /// ReelSequenceを再生する
        /// </summary>
        /// <returns>再生タスク</returns>
        UniTask PlayAsync();
        /// <summary>
        /// 末尾にReelSequenceを追加する
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        IReelSequence Append(IReelSequence sequence);
        /// <summary>
        /// 同時に実行するReelSequenceを登録する
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        IReelSequence Join(IReelSequence sequence);
        /// <summary>
        /// 末尾に待機Sequenceを追加する
        /// </summary>
        /// <param name="interval">待機時間(s)</param>
        /// <returns></returns>
        IReelSequence AppendInterval(float interval);

        /// <summary>
        /// ReelSequenceの再生完了時に実行する処理を登録する
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IReelSequence OnComplete(TweenCallback callback);
        /// <summary>
        /// ReelSequenceの再生開始時に実行する処理を登録する
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IReelSequence OnPlay(TweenCallback callback);

        /// <summary>
        /// ReelSequenceの全体の継続時間を取得する
        /// </summary>
        /// <returns></returns>
        float Duration();
    }

    internal class ReelSequence : IReelSequence
    {
        private IDictionary<RoleValue, Sequence> m_sequenceTable;

        /// <summary>
        /// 空のReelSequenceを生成する
        /// </summary>
        private ReelSequence()
        {
            m_sequenceTable = RoleValue.Every.ToDictionary(role => role, role => DOTween.Sequence());
        }
        /// <summary>
        /// RoleValueとSequenceの対応表からReelSequenceを取得する
        /// </summary>
        /// <param name="sequenceTable"></param>
        internal ReelSequence(IReadOnlyDictionary<RoleValue, Sequence> sequenceTable)
        {
            //すべてのRoleValueが揃ったDictionaryの場合はそのまま採用
            if (NullTemplate.SequenceEqual(sequenceTable)) m_sequenceTable = sequenceTable.ToDictionary();
            //揃っていなかった場合は不足分を補う
            else m_sequenceTable = sequenceTable.DictionaryCombine(Empty(), (sq, _) => sq, () => DOTween.Sequence(), () => DOTween.Sequence()).ToDictionary();
        }

        /// <summary>
        /// 空のReelSequenceを取得する
        /// </summary>
        /// <returns></returns>
        public static IReelSequence Empty() => new ReelSequence();

        /// <summary>
        /// Valueがnullのテンプレート
        /// </summary>
        private static Dictionary<RoleValue, Sequence> NullTemplate => RoleValue.Every.ToDictionary<RoleValue, RoleValue, Sequence>(r => r, _ => null);

        public Sequence this[RoleValue index]
        {
            get => m_sequenceTable[index];
            set => m_sequenceTable[index] = value;
        }

        public float Duration() => m_sequenceTable.Values.Max(sq => sq.Duration());

        public IReelSequence Append(IReelSequence sequence)
        {
            AppendBulky();

            m_sequenceTable =
                m_sequenceTable.DictionaryCombine(sequence,
                                                  (sq1, sq2) => DOTween.Sequence().Append(sq1).Append(sq2),
                                                  () => DOTween.Sequence(),
                                                  () => DOTween.Sequence())
                               .ToDictionary();
            return this;
        }

        public IReelSequence AppendInterval(float interval)
        {
            AppendBulky();

            m_sequenceTable =
                m_sequenceTable.DictionarySelect(sq => sq.AppendInterval(interval))
                               .ToDictionary();
            return this;
        }

        public IReelSequence Join(IReelSequence sequence)
        {
            m_sequenceTable =
                m_sequenceTable.DictionaryCombine(sequence,
                                                  (sq1, sq2) => DOTween.Sequence().Append(sq1).Join(sq2),
                                                  () => DOTween.Sequence(),
                                                  () => DOTween.Sequence())
                               .ToDictionary();
            return this;
        }

        public IReelSequence OnComplete(TweenCallback callback)
        {
            //Durationが最長のものにOnCompleteを設定する
            MaxDurationSequence.onComplete += callback;
            return this;
        }

        public IReelSequence OnPlay(TweenCallback callback)
        {
            MaxDurationSequence.onPlay += callback;
            return this;
        }

        public UniTask PlayAsync() =>
            m_sequenceTable.Values.Select(sq => sq.Play().AsyncWaitForCompletion().AsUniTask()).WhenAll();

        public IEnumerator<KeyValuePair<RoleValue, Sequence>> GetEnumerator() => m_sequenceTable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 各シーケンスのDurationを一番長いものに合わせる
        /// </summary>
        private void AppendBulky()
        {
            //各シーケンスの最長継続時間を取得
            float maxDuration = m_sequenceTable.Max(kvp => kvp.Value.Duration());
            //各シーケンスの継続時間を一番長いものにあわせる
            m_sequenceTable = m_sequenceTable.DictionarySelect(sq =>
            {
                float bulky = maxDuration - sq.Duration();
                if (bulky == 0f) return sq;
                else return sq.AppendInterval(bulky);
            }).ToDictionary();
        }

        /// <summary>
        /// Durationが最長のSequenceを取得する
        /// </summary>
        private Sequence MaxDurationSequence =>
            m_sequenceTable.Values
                           .OrderByDescending(sq => sq.Duration())
                           .First();

    }

    /// <summary>
    /// ReelSequenceに対する各種ユーティリティ
    /// </summary>
    public static class ReelSequenceExtensions
    {
        /// <summary>
        /// DictionaryからReelSequenceを生成する
        /// </summary>
        /// <param name="sequences"></param>
        /// <returns></returns>
        public static IReelSequence ToReelSequence(this IReadOnlyDictionary<RoleValue, Sequence> sequences) =>
            new ReelSequence(sequences);
        /// <summary>
        /// DictionaryからReelSequenceを生成する
        /// </summary>
        /// <param name="sequences"></param>
        /// <returns></returns>
        public static IReelSequence ToReelSequence(this IEnumerable<KeyValuePair<RoleValue, Sequence>> sequences) =>
            sequences.ToDictionary().ToReelSequence();
        /// <summary>
        /// SequenceをReelSequenceに変換する
        /// </summary>
        /// <param name="sequence">ソース</param>
        /// <param name="roleAs">ソースSequenceをどのRoleにアタッチするか</param>
        /// <returns></returns>
        public static IReelSequence AsReelSequence(this Sequence sequence, RoleValue roleAs)
        {
            IReelSequence sq = ReelSequence.Empty();
            sq[roleAs] = sequence;
            return sq;
        }
    }
}