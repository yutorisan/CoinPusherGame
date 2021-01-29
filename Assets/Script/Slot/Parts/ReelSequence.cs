using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace MedalPusher.Slot
{
    public class ReelSequence
    {
        private IReadOnlyList<Sequence> m_sequences;

        public ReelSequence(IEnumerable<Sequence> sequences)
        {
            m_sequences = sequences.ToList();
        }

        public UniTask Play()
        {
            return UniTask.WhenAll(m_sequences.Select(sq => sq.Play().AsyncWaitForCompletion().AsUniTask()));
        }

        public ReelSequence Append(ReelSequence appendSequence)
        {
            this.m_sequences = this.m_sequences.Zip(appendSequence.m_sequences, (sq1, sq2) => sq1.Append(sq2)).ToList();
            return this;
        }
        //public ReelSequence AppendIf(bool condition, ReelSequence appendSequence)
        //{
        //    if (condition) return this.Append(appendSequence);
        //    else return this;
        //}
    }

    public static class ReelSequenceExtensions
    {
        public static ReelSequence ToReelSequence(this IEnumerable<Sequence> sequences) =>
            new ReelSequence(sequences);
    }
}