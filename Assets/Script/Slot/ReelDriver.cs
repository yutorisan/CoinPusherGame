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

namespace MedalPusher.Slot
{
    /// <summary>
    /// 各リールの動きを制御することができる
    /// </summary>
    public interface IReelDriver
    {
        /// <summary>
        /// 指定したリールの演出に従って、リールを制御します。
        /// </summary>
        /// <param name="production">リールの演出</param>
        /// <returns>リール制御の完了通知</returns>
        UniTask ControlBy(ReelProduction production);
    }
    /// <summary>
    /// 各リールの動きを制御する
    /// </summary>
    public class ReelDriver : SerializedMonoBehaviour, IReelDriver
    {
        /// <summary>
        /// Reelの初期状態から、0を全面に持ってくるまでの補正角度
        /// </summary>
        private static readonly Angle FrontAngle = Angle.FromDegree(-80);
        /// <summary>
        /// Reelの半径
        /// </summary>
        private static readonly UniReadOnly<float> Radius = new UniReadOnly<float>(isOverwriteIgnoreMode: true);
        /// <summary>
        /// このReelに属するRoleの数
        /// </summary>
        private readonly UniReadOnly<int> RoleCount = new UniReadOnly<int>();

        [SerializeField]
        private IReelOperation m_reelOperation;
        /// <summary>
        /// スロットの回転半径
        /// </summary>
        [SerializeField]
        private float m_radius;

        /// <summary>
        /// 各Roleを動かすモジュール
        /// </summary>
        private readonly UniReadOnly<IReadOnlyDictionary<RoleValue, RoleDriver>> m_roleDrivers = new UniReadOnly<IReadOnlyDictionary<RoleValue, RoleDriver>>();
        /// <summary>
        /// 正面に持ってくる役と各役の角度テーブル
        /// </summary>
        private readonly UniReadOnly<FrontRoleAndEachAngleTable> m_frontroleAngleTable = new UniReadOnly<FrontRoleAndEachAngleTable>();

        // Start is called before the first frame update
        void Start()
        {
            RoleCount.Initialize(m_reelOperation.RoleOperations.Count);

            Radius.Initialize(m_radius);
            //RoleCount.Initialize(m_reelOperation.Roles.Count);
            m_frontroleAngleTable.Initialize(new FrontRoleAndEachAngleTable(m_reelOperation.RoleOperations.Select(ope => ope.Value).ToList()));

            //このReelが受け持つ各RoleからRoleDriverを生成して格納する
            m_roleDrivers.Initialize(
                m_reelOperation.RoleOperations
                               .Select((ope, index) => new { ope, index })
                               .ToDictionary(pair => pair.ope.Value,
                                             pair => new RoleDriver(pair.ope, (Angle.Round / RoleCount * pair.index + FrontAngle).PositiveNormalize())));                //indexの順番に均等に円形配置
                                                                                                                                                                         //+FrontAngleは正面に何かしらのRoleをもってくるため
        }


        public UniTask ControlBy(ReelProduction production)
        {
            //各Roleに対するノーマルの回転シーケンスを取得
            var normalSq = GetRollAndStopNormalSequences(production.ReelScenario.FirstRoleValue, 5, 3, 2, 2);
            //各Roleに対するリーチ演出シーケンスを取得
            var reachSq = GetReachSequences(production, 0.5f, 0.5f);
            //ノーマルシーケンスのあとにリーチ演出シーケンスを合成して再生する。再生完了通知を返す。
            return UniTask.WhenAll(normalSq.Zip(reachSq, (nSq, rSq) => nSq.Append(rSq))
                                           .Select(sq => sq.Play().AsyncWaitForCompletion().AsUniTask()));
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
        private IEnumerable<Sequence> GetRollAndStopNormalSequences(RoleValue stopRole, int laps, float maxrps, float accelDutation, float deceleDuration)
        {
            //最終的に表示する状態の各役のAngleを取得
            var angleTable = m_frontroleAngleTable.Value.GetAngleTable(stopRole);
            //すべてのRoleの制御完了通知を返す
            return 
                m_roleDrivers.Value.Values
                             .Select(driver => DOTween.Sequence()
                                                      .Append(driver.AccelerationRotate(accelDutation, maxrps))
                                                      .Append(driver.LinearRotate(laps, maxrps))
                                                      .Append(driver.DecelerateAndStopAbsAngle(angleTable[driver.RoleValue], deceleDuration, maxrps)));
        }

        /// <summary>
        /// リーチ演出を行う、各Roleに対するシーケンスを取得する
        /// </summary>
        /// <param name="startRole">リーチ演出を開始するときに正面に出ているRole</param>
        /// <param name="endRole">リーチ演出を完了すときに正面に出ているRole</param>
        /// <param name="switchDuration">Roleが切り替わるアニメーションの継続時間</param>
        /// <param name="interval">Roleが切り替わるアニメーション同士の待機時間間隔</param>
        /// <returns>各Roleに対するリーチ演出シーケンス</returns>
        private IEnumerable<Sequence> GetReachSequences(ReelProduction production, float switchDuration, float interval)
        {
            //リーチじゃなければ空のシーケンスを返す
            if (!production.ReelScenario.IsReachReelScenario) return Enumerable.Range(1, RoleCount).Select(_ => DOTween.Sequence());

            return m_roleDrivers.Value.Values
                                .Select(driver => GetRoleSequenceOnReachProduction(driver));

            //各Roleのリーチ演出シーケンスを取得する
            Sequence GetRoleSequenceOnReachProduction(RoleDriver driver)
            {
                Sequence sequence = DOTween.Sequence();
                foreach (var role in RoleValue.ForwardLoops(production.ReelScenario.FirstRoleValue, production.ReelScenario.AfterReachRole.Value))
                {
                    var angle = m_frontroleAngleTable.Value.GetAngleTable(role)[driver.RoleValue];
                    sequence = sequence.Append(driver.RollAbsolutely(angle, switchDuration, AngleTweenDirection.Forward))
                                       .AppendInterval(interval);
                }
                return sequence;
            }
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

        private class RoleDriver
        {
            /// <summary>
            /// 正面からの表示範囲角度
            /// </summary>
            private static readonly Angle RoleDisplayAngle = Angle.FromDegree(90);
            private IRoleOperation m_operation;
            /// <summary>
            /// 初期状態の角度
            /// </summary>
            private readonly Angle m_initialAngle;
            /// <summary>
            /// 初期状態の座標
            /// </summary>
            private readonly Vector3 m_initialPosition;
            /// <summary>
            /// 現在の角度
            /// </summary>
            private Angle m_nowAngle;

            ///// <summary>
            ///// 所属するReelにおけるこのRoleの順番
            ///// </summary>
            //public int Index { get; }
            /// <summary>
            /// このDriverが操るRoleの値
            /// </summary>
            public RoleValue RoleValue => m_operation.Value;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="initialAngle">初期状態の角度</param>
            public RoleDriver(IRoleOperation operation, Angle initialAngle)
            {
                m_operation = operation;
                //Index = index;
                m_initialPosition = operation.transform.position;

                m_initialAngle = initialAngle;

                ApplyAngle(m_initialAngle);
            }

            /// <summary>
            /// 指定された回転速度まで加速するTweenを取得する
            /// </summary>
            /// <param name="laps">指定の速度に到達するまでにかける回転数</param>
            /// <param name="duration">指定の速度に到達するまでにかける秒数</param>
            /// <param name="rps">到達目標速度</param>
            /// <returns></returns>
            public Tween AccelerationRotate(float duration, float rps) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           m_nowAngle + Angle.Round * duration * rps,
                           duration)
                       .SetEase(Ease.InQuad)
                       .OnComplete(PositiveNormalize);

            /// <summary>
            /// 等速の360°回転運動を行うTweenを取得する
            /// </summary>
            /// <param name="laps"></param>
            /// <param name="rps">回転速度</param>
            /// <returns></returns>
            public Tween LinearRotate(int laps, float rps) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           m_nowAngle + Angle.Round * laps,
                           laps / rps)
                       .SetEase(Ease.Linear)
                       .OnComplete(PositiveNormalize);

            /// <summary>
            /// 減速したのちに指定の角度で停止するTweenを取得する
            /// </summary>
            /// <param name="angle">停止目標とする角度</param>
            /// <param name="duration">停止するまでにかける秒数</param>
            /// <param name="initialRps">初期状態の回転速度</param>
            /// <returns></returns>
            public Tween DecelerateAndStopAbsAngle(Angle angle, float duration, float initialRps) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           angle + Angle.Round * duration * initialRps / 2,
                           duration)
                       .SetEase(Ease.OutQuad)
                       .OnComplete(PositiveNormalize);

            /// <summary>
            /// Reelを指定の角度まで回転させるTweenを取得する
            /// </summary>
            /// <param name="angle">絶対的な角度</param>
            /// <param name="duration"></param>
            public Tween RollAbsolutely(Angle angle, float duration, AngleTweenDirection direction = AngleTweenDirection.Both) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           angle,
                           duration)
                       .OnComplete(PositiveNormalize)
                       .SetOptions(direction);

            /// <summary>
            /// 現在の角度から、任意の角度ぶんReelを回転させる
            /// </summary>
            /// <param name="angle">何度回転させるか</param>
            /// <param name="duration"></param>
            public Tween RollRelatively(Angle angle, float duration, AngleTweenDirection direction = AngleTweenDirection.Both) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           m_nowAngle + angle,
                           duration)
                       .OnComplete(PositiveNormalize)
                       .SetOptions(direction);

            /// <summary>
            /// 指定の角度にRoleを回転移動する
            /// </summary>
            /// <param name="targetAngle"></param>
            private void ApplyAngle(Angle targetAngle)
            {
                //Roleのpositionを回転させる
                m_operation.transform.position = new Vector3(
                    0,
                    Radius * Mathf.Cos(targetAngle.TotalRadian),
                    Radius * Mathf.Sin(targetAngle.TotalRadian)) + m_initialPosition;
                //透明度を更新する
                m_operation.ChangeOpacity(getOpacity());
                //テーブルを更新
                m_nowAngle = targetAngle;
                if (double.IsNaN(m_nowAngle.TotalDegree)) print("NaNになった！！");
                

                float getOpacity()
                {
                    //正面の角度からの差分の絶対値を算出
                    var diffOfFront = (FrontAngle - targetAngle.Normalize()).Absolute();

                    //表示範囲より外側は不透明度0
                    if (diffOfFront > RoleDisplayAngle) return 0;
                    //内側なら外に行くたびに透明になる
                    else return 1 - diffOfFront.TotalDegree / RoleDisplayAngle.TotalDegree;
                }
            }

            /// <summary>
            /// 現在の角度を正の角度で正規化するDOTween用コールバック処理
            /// </summary>
            private TweenCallback PositiveNormalize => () => m_nowAngle = m_nowAngle.PositiveNormalize();
            /// <summary>
            /// 現在のAngleをセットするDOTween用デフォルトゲッター
            /// </summary>
            private DOGetter<Angle> NowAngleGetter => () => m_nowAngle;
            /// <summary>
            /// 受け取ったAngleをそのままApplyするDOTween用デフォルトセッター
            /// </summary>
            private DOSetter<Angle> ApplyAngleSetter => angle => ApplyAngle(angle);
        }
    }
}