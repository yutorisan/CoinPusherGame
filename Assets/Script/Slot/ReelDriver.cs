using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.MPE;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Extensions;
using Zenject;

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
        /// <param name="production"></param>
        void ControlBy(ProductionPart production);
    }
    /// <summary>
    /// 各リールの動きを制御する
    /// </summary>
    public class ReelDriver : SerializedMonoBehaviour, IReelDriver
    {
        /// <summary>
        /// Reelの初期状態から、0を全面に持ってくるまでの補正角度
        /// </summary>
        private static readonly Angle FrontAngle = Angle.FromDegree(-60);
        /// <summary>
        /// このReelに属するRoleの数
        /// </summary>
        private static readonly UniReadOnly<int> RoleCount = new UniReadOnly<int>(isOverwriteIgnoreMode: true);
        /// <summary>
        /// Reelの半径
        /// </summary>
        private static readonly UniReadOnly<float> Radius = new UniReadOnly<float>(isOverwriteIgnoreMode: true);

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
        private UniReadOnly<IReadOnlyDictionary<RoleValue, RoleDriver>> m_roleDrivers = new UniReadOnly<IReadOnlyDictionary<RoleValue, RoleDriver>>();

        private IReadOnlyDictionary<RoleValue, Angle> GetBringToFrontAngleTable(RoleValue wantToFrontRole)
        {
            var frontIndex = m_roleDrivers.Value[wantToFrontRole].Index;

            return m_roleDrivers.Value
                                .Select(kvp => new { rolev = kvp.Key, driver = kvp.Value })
                                .Select(pair => new
                                {
                                    indexDiff = pair.driver.Index - frontIndex, //FrontのRoleと各RoleとのIndexの差分を算出
                                    pair.rolev
                                })
                                .Select(pair => new
                                {
                                    angle = Angle.Round / RoleCount * pair.indexDiff + FrontAngle + Angle.Round, //算出した差分によってあるべき角度を算出
                                    pair.rolev
                                })
                                .ToDictionary(pair => pair.rolev,
                                              pair => pair.angle);
        }

        // Start is called before the first frame update
        void Start()
        {
            Radius.Initialize(m_radius);
            RoleCount.Initialize(m_reelOperation.Roles.Count);

            //このReelが受け持つ各RoleからRoleDriverを生成して格納する
            m_roleDrivers.Initialize(
                m_reelOperation.Roles
                               .Select((ope, index) => new { ope, index })
                               .ToDictionary(pair => pair.ope.Value,
                                             pair => new RoleDriver(pair.ope, pair.index)));
        }

        public void ControlBy(ProductionPart production)
        {
            RollAndStop(production.DisplayRole, 5, 3, 2, 2);
        }

        /// <summary>
        /// 停止状態からスロットを回転させ、指定の位置で停止する
        /// </summary>
        /// <param name="stopRole">停止する役柄</param>
        /// <param name="laps">等速回転時の回転数</param>
        /// <param name="maxrps">最大回転速度(rps)</param>
        /// <param name="accelDutation">加速時間(s)</param>
        /// <param name="deceleDuration">減速時間(s)</param>
        [Button("回転後に指定位置で停止")]
        private void RollAndStop(RoleValue stopRole, int laps, float maxrps, float accelDutation, float deceleDuration)
        {
            var table = GetBringToFrontAngleTable(stopRole);

            m_roleDrivers.Value.Values
                         .Select(driver => DOTween.Sequence()
                                                  .Append(driver.AccelerationRotate(accelDutation, maxrps))
                                                  .Append(driver.LinearRotate(laps, maxrps))
                                                  .Append(driver.DecelerateAndStopAbsAngle(table[driver.RoleValue], deceleDuration, maxrps)))
                         .ForEach(sq => sq.Play());
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

            /// <summary>
            /// 所属するReelにおけるこのRoleの順番
            /// </summary>
            public int Index { get; }
            /// <summary>
            /// このDriverが操るRoleの値
            /// </summary>
            public RoleValue RoleValue => m_operation.Value;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="totalRoleCount">Roleの総数</param>
            public RoleDriver(IRoleOperation operation, int index)
            {
                m_operation = operation;
                Index = index;
                m_initialPosition = operation.transform.position;
                //indexの順番に均等に円形配置
                //+FrontAngleは正面に何かしらのRoleをもってくるため
                m_initialAngle = (Angle.Round / RoleCount * index + FrontAngle).PositiveNormalize(); 

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
            public Tween RollAbsolutely(Angle angle, float duration) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           angle,
                           duration)
                       .OnComplete(PositiveNormalize);

            /// <summary>
            /// 現在の角度から、任意の角度ぶんReelを回転させる
            /// </summary>
            /// <param name="angle">何度回転させるか</param>
            /// <param name="duration"></param>
            public Tween RollRelatively(Angle angle, float duration) =>
                DOTween.To(AnglePlugin.Instance,
                           NowAngleGetter,
                           ApplyAngleSetter,
                           m_nowAngle + angle,
                           duration)
                       .OnComplete(PositiveNormalize);

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