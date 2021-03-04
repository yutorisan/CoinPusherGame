using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Permissions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityUtility.Linq;

namespace MedalPusher.Slot
{
    public interface IReelSequenceData : IEnumerable<KeyValuePair<RoleValue, Sequence>>
    {
        Sequence this[RoleValue index] { get; set; }
    }
    public interface IReelSequence : IReelSequenceData
    {
        /// <summary>
        /// ReelSequenceを再生する
        /// </summary>
        /// <returns>再生完了通知</returns>
        UniTask PlayAsync();
        IReelSequence Append(IReelSequence sequence);
        IReelSequence Join(IReelSequence sequence);
        IReelSequence AppendInterval(float interval);
        float Duration();
    }

    public class ReelSequence : IReelSequence
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
        public ReelSequence(IReadOnlyDictionary<RoleValue, Sequence> sequenceTable)
        {
            //すべてのRoleValueが揃ったDictionaryの場合はそのまま採用
            if (NullTemplate.SequenceEqual(sequenceTable)) m_sequenceTable = sequenceTable.ToDictionary();
            //揃っていなかった場合は不足分を補う
            else m_sequenceTable = sequenceTable.DictionaryCombine(Empty(), (sq, _) => sq, DOTween.Sequence(), null).ToDictionary();
        }

        /// <summary>
        /// 空のReelSequenceを取得する
        /// </summary>
        /// <returns></returns>
        public static IReelSequence Empty() => new ReelSequence();
        ///// <summary>
        ///// ReelSequenceの元となる空のテンプレートDictionaryを取得する
        ///// </summary>
        //public static Dictionary<RoleValue, Sequence> Template => RoleValue.Every.ToDictionary(role => role, role => DOTween.Sequence());

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
            return m_sequenceTable.DictionaryCombine(sequence,
                                                    　(sq1, sq2) => sq1.Append(sq2),
                                                    　DOTween.Sequence(),
                                                    　DOTween.Sequence())
                                  .ToReelSequence();
        }

        public IReelSequence AppendInterval(float interval)
        {
            AppendBulky();
            return m_sequenceTable.DictionarySelect(sq => sq.AppendInterval(interval))
                                  .ToReelSequence();
        }

        public IReelSequence Join(IReelSequence sequence) =>
            m_sequenceTable.DictionaryCombine(sequence,
                                              (sq1, sq2) => sq1.Join(sq2),
                                              DOTween.Sequence(),
                                              DOTween.Sequence())
                           .ToReelSequence();

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
    }

    //public abstract class ReelSequence : IReelSequence
    //{
    //    protected IReelSequenceData m_sequenceData;
    //    protected ReelSequenceNode m_nextSequence;
    //    protected ReelSequenceRoot m_root;

    //    /// <summary>
    //    /// 予めすべてのRoleValueをキーとしたメンバがAddされた、ReelSequenceの素となる空のDictionaryを取得する
    //    /// </summary>
    //    public static Dictionary<RoleValue, Sequence> TemplateDictionary =>
    //        RoleValue.Every.ToDictionary<RoleValue, RoleValue, Sequence>(role => role, _ => DOTween.Sequence());

    //    public ReelSequenceRoot Root => m_root;

    //    /// <summary>
    //    /// 空のReelSequenceを取得する
    //    /// </summary>
    //    /// <returns></returns>
    //    public static IReelSequence Empty() => new ReelSequenceRoot(TemplateDictionary);

    //    public Sequence this[RoleValue index]
    //    {
    //        get => m_sequenceData[index];
    //        set => m_sequenceData[index] = value;
    //    }

    //    public float Duration() => m_sequenceData.Max(kvp => kvp.Value.Duration());

    //    public virtual UniTask PlayAsync() =>
    //        UniTask.WhenAll(m_sequenceData.SelectValue()
    //                                      .Select(sq => sq.Play()
    //                                                      .AsyncWaitForCompletion()
    //                                                      .AsUniTask()))
    //               .ContinueWithIf(m_nextSequence != null,
    //                               () => m_nextSequence.PlayNextAsync());

    //    public IReelSequence Append(IReelSequence sequence)
    //    {
    //        m_nextSequence = sequence.Root.AsNode(this.Root);
    //        return m_nextSequence;
    //    }
    //    public IReelSequence Join(IReelSequence sequence)
    //    {
    //        m_sequenceData = new ReelSequenceData(m_sequenceData.DictionaryZip(sequence, (sq1, sq2) => sq1.Join(sq2)).ToDictionary());
    //        return this;
    //    }
    //    public IReelSequence AppendInterval(float interval)
    //    {
    //        m_sequenceData = new ReelSequenceData(m_sequenceData.DictionarySelect(sq => sq.AppendInterval(interval)).ToDictionary());
    //        return this;
    //    }

    //    public IEnumerator<KeyValuePair<RoleValue, Sequence>> GetEnumerator() => m_sequenceData.GetEnumerator();
    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}




    //public class ReelSequenceRoot : ReelSequence
    //{
    //    internal ReelSequenceRoot(IDictionary<RoleValue, Sequence> sequences)
    //    {
    //        m_root = this;
    //        m_sequenceData = new ReelSequenceData(sequences);
    //    }

    //    public ReelSequenceNode AsNode(ReelSequenceRoot root)
    //    {
    //        return new ReelSequenceNode(root, this, m_nextSequence);
    //    }
    //}




    //public class ReelSequenceNode : ReelSequence
    //{

    //    internal ReelSequenceNode(ReelSequenceRoot root, IReelSequence mySequence, ReelSequenceNode next)
    //    {
    //        m_root = root;
    //        m_sequenceData = mySequence;
    //        m_nextSequence = next;
    //    }
    //    public override UniTask PlayAsync() => m_root.PlayAsync();

    //    public UniTask PlayNextAsync() => base.PlayAsync();

    //}



    //public class ReelSequenceData : IReelSequenceData
    //{
    //    private IDictionary<RoleValue, Sequence> m_sequenceTable;

    //    public ReelSequenceData(IDictionary<RoleValue, Sequence> sequences)
    //    {
    //        m_sequenceTable = sequences;
    //    }

    //    public Sequence this[RoleValue index]
    //    {
    //        get => m_sequenceTable[index];
    //        set => m_sequenceTable[index] = value;
    //    }

    //    public IEnumerator<KeyValuePair<RoleValue, Sequence>> GetEnumerator() => m_sequenceTable.GetEnumerator();
    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}

    //public class ReelSequence
    //{
    //    private IDictionary<RoleValue, Sequence> m_sequences;
    //    private ReelSequence m_rootSequence;
    //    private ReelSequence m_thisSequence;
    //    private ReelSequence m_nextSequence;

    //    /// <summary>
    //    /// 予めすべてのRoleValueをキーとしたメンバがAddされた、ReelSequenceの素となる空のDictionaryを取得する
    //    /// </summary>
    //    public static Dictionary<RoleValue, Sequence> TemplateDictionary =>
    //        RoleValue.Every.ToDictionary<RoleValue, RoleValue, Sequence>(role => role, _ => null);
    //    /// <summary>
    //    /// 空のReelSequenceを取得する
    //    /// </summary>
    //    /// <returns></returns>
    //    public static ReelSequence Empty() => new ReelSequence(TemplateDictionary);

    //    public ReelSequence(IDictionary<RoleValue, Sequence> sequences)
    //    {
    //        m_sequences = sequences;
    //        m_nextSequence = this;
    //    }
    //    private ReelSequence(ReelSequence previousSequence, ReelSequence thisSequence)
    //    {
    //        m_sequences = thisSequence;
    //        m_rootSequence = previousSequence;
    //    }

    //    public Sequence this[RoleValue index]
    //    {
    //        get => m_sequences[index];
    //        set => m_sequences[index] = value;
    //    }

    //    public float Duration => m_sequences.Max(table => table.Value.Duration());

    //    public UniTask PlayAsync()
    //    {
    //        return UniTask.WhenAll(m_sequences.SelectValue()
    //                                          .ExcludeNull()
    //                                          .Select(sq => sq.Play()
    //                                                          .AsyncWaitForCompletion()
    //                                                          .AsUniTask()
    //                                                          .ContinueWith(() =>
    //                                                          {
    //                                                              if (m_nextSequence is null) return UniTask.CompletedTask;
    //                                                              else return m_nextSequence.PlayAsync();
    //                                                          })));
    //    }

    //    public ReelSequence Append(ReelSequence appendSequence)
    //    {
    //        return new ReelSequence(appendedSq);
    //    }

    //    public ReelSequence Join(ReelSequence joinSequence)
    //    {
    //        m_sequences = m_sequences.DictionaryZip(joinSequence.m_sequences,
    //                                                (sq1, sq2) => SequenceCalc(sq1, sq2, (sqA, sqB) => sqA.Join(sqB)))
    //                                 .ToDictionary();
    //        return this;
    //    }

    //    public ReelSequence AppendInterval(float interval)
    //    {
    //        m_sequences = m_sequences.DictionarySelect(sq => sq.AppendInterval(interval)).ToDictionary();
    //        return this;
    //    }

    //    private Sequence SequenceCalc(Sequence sq1, Sequence sq2, Func<Sequence, Sequence, Sequence> sqCalcMethod)
    //    {
    //        if (sq1 == null && sq2 == null) return null; //null同士のAppendはnull
    //        if (sq1 != null && sq2 == null) return sq1;  //一方だけnullでないなら   
    //        if (sq1 == null && sq2 != null) return sq2;  //nullでない方を返す
    //        return sqCalcMethod(sq1, sq2);               //両方ともSqなら演算結果を返す
    //    }
    //}

    public static class ReelSequenceExtensions
    {
        public static IReelSequence ToReelSequence(this IReadOnlyDictionary<RoleValue, Sequence> sequences) =>
            new ReelSequence(sequences);
        public static IReelSequence ToReelSequence(this IEnumerable<KeyValuePair<RoleValue, Sequence>> sequences) =>
            sequences.ToDictionary().ToReelSequence();
        /// <summary>
        /// SequenceをReelSequenceに変換します
        /// 指定したRole以外はnullが入ります
        /// </summary>
        /// <param name="sequence">ソース</param>
        /// <param name="roleAs">どのRoleのSequenceか</param>
        /// <returns></returns>
        public static IReelSequence AsReelSequence(this Sequence sequence, RoleValue roleAs)
        {
            IReelSequence sq = ReelSequence.Empty();
            sq[roleAs] = sequence;
            return sq;
        }
    }
}