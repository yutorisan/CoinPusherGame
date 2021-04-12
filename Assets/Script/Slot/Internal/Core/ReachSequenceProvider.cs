using System.Collections.Generic;
using UnityUtility;
using DG.Tweening;
using UnityEngine;
using System;
using System.Collections;
using MedalPusher.Utils;

namespace MedalPusher.Slot.Internal.Core
{
    public partial class ReelSequenceProvider
    {
        /// <summary>
        /// リーチ時の拮抗演出シーケンスを提供できる
        /// </summary>
        private interface IReachAntagonistSequenceProvider
        {
            /// <summary>
            /// 拮抗演出シーケンスを取得する
            /// </summary>
            /// <param name="antagonist">取得する拮抗演出タイプ</param>
            /// <param name="property">取得する拮抗演出のプロパティ</param>
            /// <returns></returns>
            IReelSequence GetAntagonistSequence(AntagonistType antagonist, AntagonistSequenceProperty property);
        }

        /// <summary>
        /// リーチ時の拮抗演出シーケンスを提供する
        /// </summary>
        private class ReachAntagonistSequenceProvider : IReachAntagonistSequenceProvider
        {
            /// <summary>
            /// 各拮抗演出タイプに対応する、拮抗演出クリエイターの対応テーブル
            /// </summary>
            private readonly IReadOnlyDictionary<AntagonistType, IAntagonistSequenceCreator> antagonistSqCreatorTable;

            public ReachAntagonistSequenceProvider(ReelSequenceProvider parent)
            {
                //各拮抗演出に対応するクリエイターオブジェクトを生成して格納する
                antagonistSqCreatorTable = new Dictionary<AntagonistType, IAntagonistSequenceCreator>()
                {
                    {AntagonistType.Type1, new Type1AntagonistSqCreator(parent) },
                    {AntagonistType.Type2, new Type2AntagonistSqCreator(parent) },
                    {AntagonistType.Type3, new Type3AntagonistSqCreator(parent) },
                };
            }

            public IReelSequence GetAntagonistSequence(AntagonistType antagonist, AntagonistSequenceProperty property)
            {
                return antagonistSqCreatorTable[antagonist].Create(property);
            }

            #region AntagonistSequenceCreator
            /// <summary>
            /// 拮抗演出シーケンスを生成できる
            /// </summary>
            private interface IAntagonistSequenceCreator
            {
                /// <summary>
                /// 拮抗演出シーケンスを生成する
                /// </summary>
                /// <param name="property"></param>
                /// <returns></returns>
                IReelSequence Create(AntagonistSequenceProperty property);
            }

            /// <summary>
            /// 拮抗演出シーケンス生成クラスのベース
            /// </summary>
            private abstract class AntagonistSqCreatorBase : IAntagonistSequenceCreator
            {
                protected readonly ReelSequenceProvider sqProvider;
                protected readonly Angle roleIntervalAngle;
                public AntagonistSqCreatorBase(ReelSequenceProvider parent)
                {
                    sqProvider = parent;
                    roleIntervalAngle = parent.RoleIntervalAngle;
                }
                public abstract IReelSequence Create(AntagonistSequenceProperty property);
            }

            /// <summary>
            /// 拮抗演出１：行ったり来たりする演出
            /// </summary>
            private class Type1AntagonistSqCreator : AntagonistSqCreatorBase
            {
                public Type1AntagonistSqCreator(ReelSequenceProvider parent) : base(parent) { }
                public override IReelSequence Create(AntagonistSequenceProperty property)
                {
                    //リール全体が行ったり来たりするシーケンスを生成
                    var antagonistSq =
                        ReelSequence.Empty()
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * .5f, .3f)) //.5
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * -.5f, .3f)) // 0
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * .7f, .3f)) //.7
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * -.5f, .3f)) //.2
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * .6f, .3f)) //.8
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * -.4f, .3f)) //.4
                                    .Append(sqProvider.CreateRollSequence(roleIntervalAngle * .5f, .3f)) //.9
                                    .AppendInterval(1f);


                    //最後に、当たりかハズレかによって、リールを進めるか戻すかのシーケンスを追加する
                    if (property.IsOffenserWin)
                    {
                        antagonistSq.Append(sqProvider.CreateRollSequence(roleIntervalAngle * .1f, .1f));
                    }
                    else
                    {
                        antagonistSq.Append(sqProvider.CreateRollSequence(roleIntervalAngle * -.9f, .4f));
                    }

                    return antagonistSq;
                }
            }

            /// <summary>
            /// 拮抗演出２：中央で止まって振動する
            /// </summary>
            private class Type2AntagonistSqCreator : AntagonistSqCreatorBase
            {
                public Type2AntagonistSqCreator(ReelSequenceProvider parent) : base(parent)
                {
                }

                public override IReelSequence Create(AntagonistSequenceProperty property) =>
                    ReelSequence.Empty()
                                //中央までリールを回転させて
                                .Append(sqProvider.CreateRollSequence(roleIntervalAngle * .5f, 1.5f))
                                //振動させて
                                .Append(sqProvider.Create(t => t.DOShakePosition(3, 0.02f).ToSequence(), property.Offenser, property.Defenser))
                                //当たりなら進める、ハズレなら戻す
                                .Append(sqProvider.CreateRollSequence(property.IsOffenserWin ? roleIntervalAngle * .5f : roleIntervalAngle * -.5f, .5f));
            }

            /// <summary>
            /// 拮抗演出３：前面に出てきて戦う
            /// </summary>
            private class Type3AntagonistSqCreator : AntagonistSqCreatorBase
            {
                /// <summary>
                /// 戦闘の中心座標
                /// </summary>
                private static readonly Vector3 fightCenterPosition = new Vector3(0, 0.377383053f, -0.35f);
                /// <summary>
                /// 戦闘の中心座標から、両者のベース位置までの距離
                /// </summary>
                private static readonly float centerToFighterLength = .15f;

                public Type3AntagonistSqCreator(ReelSequenceProvider parent) : base(parent)
                {
                }

                public override IReelSequence Create(AntagonistSequenceProperty property)
                {
                    //戦うベースポジション
                    var leftBasePosition = fightCenterPosition.PlusX(-centerToFighterLength);
                    var rightBasePosition = fightCenterPosition.PlusX(centerToFighterLength);

                    //OffenserがDefenserにツンツンと攻撃するようなシーケンスを取得するテーブル
                    Dictionary<RoleValue, Func<RoleTweenProvider, Sequence>> firstAntagonistSelectorTable =
                        new Dictionary<RoleValue, Func<RoleTweenProvider, Sequence>>()
                        {
                            [property.Offenser] = provider =>
                                DOTween.Sequence()
                                       .Append(provider.RollMultiple(.7f, .2f))                     //速くぶつかって
                                       .Append(provider.RollMultiple(-.7f, .8f))                    //ゆっくり戻って
                                       .Append(provider.RollMultiple(.8f, .2f))                     //速くぶつかって
                                       .Append(provider.RollMultiple(-.8f, .8f))                    //ゆっくり戻って
                                       .Append(provider.RollMultiple(.9f,  .1f))                    //速くぶつかって
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

                    //戦闘フィールドのX方向の長さに対してmultiple倍だけ動くTweenを取得する
                    Tween FightTweenX(Transform transform, float multiple, float duration) =>
                        transform.DOMoveX(centerToFighterLength * multiple, duration).SetRelative();
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// 拮抗演出の種類
    /// </summary>
    public enum AntagonistType
    {
        Type1,
        Type2,
        Type3
    }

    /// <summary>
    /// 拮抗演出を生成するために必要な情報
    /// </summary>
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