using System.Collections.Generic;
using UnityUtility;
using DG.Tweening;

namespace MedalPusher.Slot.Sequences
{
    public partial class ReelSequenceProvider
    {
        private interface IReachAntagonistSequenceProvider
        {
            IReelSequence GetAntagonistSequence(AntagonistType antagonist, AntagonistSequenceProperty property);
        }
        private class ReachAntagonistSequenceProvider : IReachAntagonistSequenceProvider
        {
            private readonly ReelSequenceProvider m_parent;
            private readonly IReadOnlyDictionary<AntagonistType, IAntagonistSequenceCreator> m_sqCreatorTable;
            public ReachAntagonistSequenceProvider(ReelSequenceProvider parent)
            {
                m_parent = parent;
                m_sqCreatorTable = new Dictionary<AntagonistType, IAntagonistSequenceCreator>()
                {
                    {AntagonistType.Type1, new Type1AntagonistSqCreator(parent) },
                    {AntagonistType.Type2, new Type2AntagonistSqCreator(parent) }
                };
            }

            public IReelSequence GetAntagonistSequence(AntagonistType antagonist, AntagonistSequenceProperty property)
            {
                return m_sqCreatorTable[antagonist].Create(property);
            }

            private interface IAntagonistSequenceCreator
            {
                IReelSequence Create(AntagonistSequenceProperty property);
            }
            private abstract class AntagonistSqCreatorBase : IAntagonistSequenceCreator
            {
                protected readonly ReelSequenceProvider sqProvider;
                protected readonly Angle _roleInterval;
                public AntagonistSqCreatorBase(ReelSequenceProvider parent)
                {
                    sqProvider = parent;
                    _roleInterval = parent.RoleIntervalAngle;
                }
                public abstract IReelSequence Create(AntagonistSequenceProperty property);
            }
            private class Type1AntagonistSqCreator : AntagonistSqCreatorBase
            {
                public Type1AntagonistSqCreator(ReelSequenceProvider parent) : base(parent) { }
                public override IReelSequence Create(AntagonistSequenceProperty property)
                {
                    var antagonistSq =
                    ReelSequence.Empty()
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .5f, .3f)) //.5
                                .Append(sqProvider.CreateRollSequence(_roleInterval * -.5f, .3f)) // 0
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .7f, .3f)) //.7
                                .Append(sqProvider.CreateRollSequence(_roleInterval * -.5f, .3f)) //.2
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .6f, .3f)) //.8
                                .Append(sqProvider.CreateRollSequence(_roleInterval * -.4f, .3f)) //.4
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .5f, .3f)) //.9
                                .AppendInterval(1f);


                    if (property.IsOffenserWin)
                    {
                        antagonistSq.Append(sqProvider.CreateRollSequence(_roleInterval * .1f, .1f));
                    }
                    else
                    {
                        antagonistSq.Append(sqProvider.CreateRollSequence(_roleInterval * -.9f, .4f));
                    }

                    return antagonistSq;
                }
            }
            private class Type2AntagonistSqCreator : AntagonistSqCreatorBase
            {
                public Type2AntagonistSqCreator(ReelSequenceProvider parent) : base(parent)
                {
                }

                public override IReelSequence Create(AntagonistSequenceProperty property)
                {
                    return
                    ReelSequence.Empty()
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .5f, 1.5f))
                                .Append(sqProvider.Create(t => t.DOShakePosition(3, 0.02f).ToSequence(), property.Offenser, property.Defenser))
                                .Append(sqProvider.CreateRollSequence(property.IsOffenserWin ? _roleInterval * .5f : _roleInterval * -.5f, .5f));

                    
                }
            }
        }


    }

    public enum AntagonistType
    {
        Type1,
        Type2,
    }

    public struct AntagonistSequenceProperty
    {
        /// <summary>
        /// 出目を勝ち取ろうとするRole（これが勝つと当たり）
        /// </summary>
        public RoleValue Offenser { get; set; }
        /// <summary>
        /// 現状の出目で、その座を守ろうとするRole（これが勝つとはずれ）
        /// </summary>
        public RoleValue Defenser { get; set; }
        /// <summary>
        /// Offenserが勝ち、あたりになるかどうか
        /// </summary>
        public bool IsOffenserWin { get; set; }
    }
}