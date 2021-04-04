using System.Collections.Generic;
using UnityUtility;
using DG.Tweening;
using UnityEngine;
using System;
using MedalPusher.Slot.Sequences.Core;
using System.Collections;

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
                    {AntagonistType.Type2, new Type2AntagonistSqCreator(parent) },
                    {AntagonistType.Type3, new Type3AntagonistCreator(parent, new Vector3(0,0.377383053f,-0.35f), .15f, m_parent.m_frontroleAngleTable) },
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

                public override IReelSequence Create(AntagonistSequenceProperty property) =>
                    ReelSequence.Empty()
                                .Append(sqProvider.CreateRollSequence(_roleInterval * .5f, 1.5f))
                                .Append(sqProvider.Create(t => t.DOShakePosition(3, 0.02f).ToSequence(), property.Offenser, property.Defenser))
                                .Append(sqProvider.CreateRollSequence(property.IsOffenserWin ? _roleInterval * .5f : _roleInterval * -.5f, .5f));
            }
            private class Type3AntagonistCreator : AntagonistSqCreatorBase
            {
                /// <summary>
                /// 戦闘の中心座標
                /// </summary>
                private readonly Vector3 m_fightCenterPosition;
                /// <summary>
                /// 戦闘の中心座標から、両者のベース位置までの距離
                /// </summary>
                private readonly float m_centerToFighterLength;
                private readonly FrontRoleAndEachAngleTable m_angleTable;

                public Type3AntagonistCreator(ReelSequenceProvider parent, Vector3 fightCenter, float centerToLength, FrontRoleAndEachAngleTable table) : base(parent)
                {
                    m_fightCenterPosition = fightCenter;
                    m_centerToFighterLength = centerToLength;
                    m_angleTable = table;
                }

                public override IReelSequence Create(AntagonistSequenceProperty property)
                {
                    var leftBasePosition = m_fightCenterPosition.PlusX(-m_centerToFighterLength);
                    var rightBasePosition = m_fightCenterPosition.PlusX(m_centerToFighterLength);

                    //offenserがleft, defenserがright

                    //最初の拮抗
                    Dictionary<RoleValue, Func<RoleTweenProvider, Sequence>> firstAntagonistSelectorTable =
                        new Dictionary<RoleValue, Func<RoleTweenProvider, Sequence>>()
                        {
                            //[property.Defenser] = provider =>
                            //    DOTween.Sequence()
                            //           .Append(provider.RollRelatively(-_roleInterval / 2, 1f)),
                            [property.Offenser] = provider =>
                                DOTween.Sequence()
                                       .Append(provider.RollMultiple(.7f, .2f))                     //速くぶつかって
                                       //.Append(provider.Create(t => t.DOShakePosition(.5f, .05f)))   //衝撃のエフェクトを掛けて
                                       .Append(provider.RollMultiple(-.7f, .8f))                    //ゆっくり戻って
                                       .Append(provider.RollMultiple(.8f, .2f))                     //速くぶつかって
                                       //.Append(provider.Create(t => t.DOShakePosition(.5f, .1f)))   //衝撃のエフェクトを掛けて
                                       .Append(provider.RollMultiple(-.8f, .8f))                    //ゆっくり戻って
                                       .Append(provider.RollMultiple(.9f,  .1f))                    //速くぶつかって
                                       //.Append(provider.Create(t => t.DOShakePosition(.5f, .15f)))   //衝撃のエフェクトを掛けて
                                       .Append(provider.RollMultiple(-.9f, .8f))                    //ゆっくり戻って
                        };

                    //前に出てきて対決
                    Dictionary<RoleValue, Func<Transform, Sequence>> fightSelectorTable =
                        new Dictionary<RoleValue, Func<Transform, Sequence>>()
                        {
                            [property.Defenser] = t =>
                                DOTween.Sequence()
                                       .Append(t.DOMove(rightBasePosition, .5f))                //リールからベースポジションに移動して
                                       .AppendInterval(1f)                                      //1秒待機して
                                       .Append(FightTweenX(t, -1, .2f).SetEase(Ease.InCubic))   //素早く激突して
                                       .Append(t.DOShakePosition(.5f, .2f))
                                       .Append(FightTweenX(t, .5f, .5f))                        //半分くらい戻って
                                       .Append(FightTweenX(t, -.5f, .15f).SetEase(Ease.InCubic))//また激突して
                                       .AppendInterval(.5f)
                                       .Append(FightTweenX(t, -.2f + .05f, .5f).SetEase(Ease.Linear))  //押して
                                       .Append(FightTweenX(t, .4f + .05f, 1f).SetEase(Ease.Linear))    //押されて
                                       .Append(FightTweenX(t, -.2f, .5f).SetEase(Ease.Linear)), //中央に戻って

                            [property.Offenser] = t =>
                                DOTween.Sequence()
                                       .Append(t.DOMove(leftBasePosition, .5f))
                                       .AppendInterval(1f)
                                       .Append(FightTweenX(t, 1, .2f).SetEase(Ease.InCubic))
                                       .Append(t.DOShakePosition(.5f, .2f))
                                       .Append(FightTweenX(t, -.5f, .5f))
                                       .Append(FightTweenX(t, .5f, .15f).SetEase(Ease.InCubic))
                                       .AppendInterval(.5f)
                                       .Append(FightTweenX(t, -.2f - .05f, .5f).SetEase(Ease.Linear))  //押されて
                                       .Append(FightTweenX(t, .4f +.05f, 1f).SetEase(Ease.Linear))    //押して
                                       .Append(FightTweenX(t, -.2f, .5f).SetEase(Ease.Linear)), //中央に戻って

                        };


                    return sqProvider.CreateFromProvider(firstAntagonistSelectorTable)
                                     .AppendInterval(0.5f)
                                     .Append(sqProvider.Create(fightSelectorTable))
                                     .Append(sqProvider.CreateBackToReelSequence(property.WinRole, .5f));

                    Tween FightTweenX(Transform transform, float multiple, float duration) =>
                        transform.DOMoveX(m_centerToFighterLength * multiple, duration).SetRelative();
                }
            }
        }


    }

    public enum AntagonistType
    {
        Type1,
        Type2,
        Type3
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
        /// <summary>
        /// 勝利するRole
        /// </summary>
        public RoleValue WinRole => IsOffenserWin ? Offenser : Defenser;
    }
}