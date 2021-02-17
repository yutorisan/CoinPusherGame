using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Linq;
using Zenject;
using UniRx.Diagnostics;
using Cysharp.Threading.Tasks;
using MedalPusher.Slot.Sequences.Core;

namespace MedalPusher.Slot.Sequences
{
    public interface IReelSequenceProvider
    {
        /// <summary>
        /// 通常のリール回転シーケンスを取得する
        /// </summary>
        /// <returns></returns>
        ReelSequence CreateFirstRollSequence(RoleValue stopRole, NormalRollProductionProperty prop);
        /// <summary>
        /// リーチ時の再抽選シーケンスを取得する
        /// </summary>
        /// <returns></returns>
        ReelSequence CreateReachReRollSequence(RoleValue startRole, RoleValue endValue, ReachProductionProperty prop);
        /// <summary>
        /// 当たったときの演出シーケンスを取得する
        /// </summary>
        /// <returns></returns>
        ReelSequence CreateWinningProductionSequence(RoleValue target);

        ReelSequence CreateAntagonistSequence(AntagonistType antagonist, bool isNextWin);
    }

    /// <summary>
    /// 各リールの動きを制御するシーケンスを提供する
    /// </summary>
    public partial class ReelSequenceProvider : IReelSequenceProvider
    {
        /// <summary>
        /// Reelの初期状態から、0を全面に持ってくるまでの補正角度
        /// </summary>
        private static readonly Angle FrontAngle = Angle.FromDegree(-80);
        /// <summary>
        /// このReelに属するRoleの数
        /// </summary>
        private readonly int m_roleCount;
        /// <summary>
        /// 各Roleを動かすモジュール
        /// </summary>
        private readonly IReadOnlyDictionary<RoleValue, RoleTweenProvider> m_tweenProvider;
        /// <summary>
        /// 正面に持ってくる役と各役の角度テーブル
        /// </summary>
        private readonly FrontRoleAndEachAngleTable m_frontroleAngleTable;
        /// <summary>
        /// リーチ演出の最後で拮抗する演出を提供する
        /// </summary>
        private readonly IReachAntagonistSequenceProvider m_antagonistProvider;
        /// <summary>
        /// Role間の角度
        /// </summary>
        private Angle RoleIntervalAngle => Angle.Round / m_roleCount;

        public ReelSequenceProvider(IReadOnlyCollection<IRoleOperation> operations, float reelRadius)
        {
            m_roleCount = operations.Count;

            m_frontroleAngleTable = new FrontRoleAndEachAngleTable(operations.Select(ope => ope.Value).ToList());
            m_antagonistProvider = new ReachAntagonistSequenceProvider(this);

            //このReelが受け持つ各RoleからRoleDriverを生成して格納する
            m_tweenProvider =
                operations.Select((ope, index) => (ope, initAngle: (Angle.Round / m_roleCount * index + FrontAngle).PositiveNormalize())) //indexの順番に均等に円形配置
                          .ToDictionary(pair => pair.ope.Value,
                                        pair => new RoleTweenProvider(pair.ope, pair.initAngle, reelRadius, FrontAngle));

        }

        /// <summary>
        /// 停止状態からスロットを回転させ、指定の位置で停止する、通常の各Roleに対するシーケンスを取得する
        /// </summary>
        /// <param name="stopRole">正面に停止させる役柄</param>
        /// <param name="laps">等速回転時の回転数</param>
        /// <param name="maxrps">最大回転速度(rps)</param>
        /// <param name="accelDutation">加速時間(s)</param>
        /// <param name="deceleDuration">減速時間(s)</param>
        /// <returns>各Roleに対するシーケンス</returns>
        public ReelSequence CreateFirstRollSequence(RoleValue stopRole, NormalRollProductionProperty prop) =>
            //すべてのRoleの制御完了通知を返す 
            m_tweenProvider.DictionarySelect(provider => DOTween.Sequence()
                                                                .Append(provider.AccelerationRotate(prop.AccelDuration, prop.MaxRps))
                                                                .Append(provider.LinearRotate(prop.Laps, prop.MaxRps))
                                                                .Append(provider.DecelerateAndStopAbsAngle(m_frontroleAngleTable.GetAngleTable(stopRole)[provider.RoleValue], prop.DeceleDuration, prop.MaxRps)))
                           .ToReelSequence();

        /// <summary>
        /// リーチ演出を行う、各Roleに対するシーケンスを取得する
        /// </summary>
        /// <param name="startRole">リーチ演出を開始するときに正面に出ているRole</param>
        /// <param name="endRole">リーチ演出を完了すときに正面に出ているRole</param>
        /// <param name="switchDuration">Roleが切り替わるアニメーションの継続時間</param>
        /// <param name="interval">Roleが切り替わるアニメーション同士の待機時間間隔</param>
        /// <returns>各Roleに対するリーチ演出シーケンス</returns>
        public ReelSequence CreateReachReRollSequence(RoleValue startRole, RoleValue endRole, ReachProductionProperty prop)
        {
            return m_tweenProvider.DictionarySelect(provider => GetRoleSequenceOnReachProduction(provider))
                                  .ToReelSequence();
            //各Roleのリーチ演出シーケンスを取得する
            Sequence GetRoleSequenceOnReachProduction(RoleTweenProvider provider)
            {
                Sequence sequence = DOTween.Sequence();
                foreach (var role in RoleValue.ForwardLoops(startRole, endRole))
                {
                    var angle = m_frontroleAngleTable.GetAngleTable(role)[provider.RoleValue];

                    float duration, interval;
                    //残り切り替わり回数によって各種間隔を切り替える
                    if(RoleValue.RemainCount(role, endRole) > prop.FastModeThreshold)
                    { //残り切り替わり回数がしきい値より多い
                        duration = prop.FastSwitchingDuration;
                        interval = prop.FastSwitchInterval;
                    }
                    else
                    { //残り切り替わり回数がしきい値を切った
                        duration = prop.SwitchingDuraion;
                        interval = prop.SwitchInterval;
                    }

                    sequence = sequence.Append(provider.RollAbsolutely(angle, duration, AngleTweenDirection.Forward))
                                       .AppendInterval(interval);
                }
                return sequence;
            }
        }

        public ReelSequence CreateAntagonistSequence(AntagonistType antagonist, bool isNextWin)
        {
            return m_antagonistProvider.GetAntagonistSequence(antagonist, isNextWin);
        }

        public ReelSequence CreateWinningProductionSequence(RoleValue target)
        {
            return m_tweenProvider[target].GetWinningProductionSequence().AsReelSequence(target);
        }

        public ReelSequence RelativeRollSequence(Angle rollAngle, float duration)
        {
            IEnumerable<KeyValuePair<RoleValue, Tween>> tweens;
            if (rollAngle.IsPositive)
            {
                tweens = m_tweenProvider.DictionarySelect(provider => provider.RollRelatively(rollAngle, duration, AngleTweenDirection.Forward));
            }
            else
            {
                tweens = m_tweenProvider.DictionarySelect(provider => provider.RollRelatively(rollAngle, duration, AngleTweenDirection.Backward));
            }

            return tweens.DictionarySelect(tw => DOTween.Sequence().Append(tw)).ToReelSequence();
        }

        /// <summary>
        /// 正面に持ってくる役と、そのときの各役がいるべき角度の対応テーブル
        /// </summary>
        private class FrontRoleAndEachAngleTable
        {
            /// <summary>
            /// 正面に持ってくる役と、そのときの各役の角度値のマッピングテーブル
            /// </summary>
            private readonly IReadOnlyDictionary<RoleValue, IReadOnlyDictionary<RoleValue, Angle>> m_frontroleAngleTable;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="roleIndexTable"></param>
            public FrontRoleAndEachAngleTable(IReadOnlyList<RoleValue> roleList)
            {
                m_frontroleAngleTable = roleList.ToDictionary(role => role, role => GetBringToFrontAngleTable(role));

                /// <summary>
                /// 特定のRoleを正面に表示したときの、各RoleのAngleの対応テーブルを取得する
                /// </summary>
                /// <param name="frontRole">正面に表示するRole</param>
                /// <returns></returns>
                IReadOnlyDictionary<RoleValue, Angle> GetBringToFrontAngleTable(RoleValue frontRole)
                {
                    return roleList.Select(role => new { original = role, diffIndex = frontRole.Index - role.Index }) //FrontのRoleと各RoleとのIndexの差分を算出
                                  .ToDictionary(pair => pair.original,
                                                pair => (Angle.Round / roleList.Count * pair.diffIndex + FrontAngle).PositiveNormalize()); //算出した差分によってあるべき角度を算出
                }
            }

            /// <summary>
            /// いずれかの役を正面に持ってきたときの、各役の配置角度テーブルを取得する
            /// </summary>
            /// <param name="frontAngle">正面に持ってくる役</param>
            /// <returns></returns>
            public IReadOnlyDictionary<RoleValue, Angle> GetAngleTable(RoleValue frontRole) => m_frontroleAngleTable[frontRole];
        }

    }
}