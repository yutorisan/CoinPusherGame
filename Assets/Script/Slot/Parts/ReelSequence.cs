using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityUtility.Linq;

namespace MedalPusher.Slot
{
    public class ReelSequence
    {
        private IReadOnlyDictionary<RoleValue, Sequence> m_sequences;


        public ReelSequence(IReadOnlyDictionary<RoleValue, Sequence> sequences)
        {
            m_sequences = sequences;
        }

        public UniTask PlayAsync()
        {
            return UniTask.WhenAll(m_sequences.SelectValue()
                                              .ExcludeNull()
                                              .Select(sq => sq.Play()
                                                              .AsyncWaitForCompletion()
                                                              .AsUniTask()));
        }

        public ReelSequence Append(ReelSequence appendSequence)
        {
            m_sequences = m_sequences.DictionaryZip(appendSequence.m_sequences,
                                                    (sq1, sq2) =>
                                                    {
                                                        if (sq1 == null && sq2 == null) return null; //null同士のAppendはnull
                                                        if (sq1 != null && sq2 == null) return sq1;  //一方だけnullでないなら   
                                                        if (sq1 == null && sq2 != null) return sq2;  //nullでない方を返す
                                                        else return sq1.Append(sq2);                 //両方ともSqなら普通にAppend
                                                    })
                                     .ToDictionary();
            return this;
        }

    }

    public static class ReelSequenceExtensions
    {
        public static ReelSequence ToReelSequence(this IReadOnlyDictionary<RoleValue, Sequence> sequences) =>
            new ReelSequence(sequences);
        public static ReelSequence ToReelSequence(this IEnumerable<KeyValuePair<RoleValue, Sequence>> sequences) =>
            sequences.ToDictionary().ToReelSequence();
        /// <summary>
        /// SequenceをReelSequenceに変換します
        /// 指定したRole以外はnullが入ります
        /// </summary>
        /// <param name="sequence">ソース</param>
        /// <param name="roleAs">どのRoleのSequenceか</param>
        /// <returns></returns>
        public static ReelSequence AsReelSequence(this Sequence sequence, RoleValue roleAs)
        {
            var dic = RoleValue.Every.ToDictionary<RoleValue, RoleValue, Sequence>(role => role, _ => null);
            dic[roleAs] = sequence;
            return new ReelSequence(dic);
        }
    }
}