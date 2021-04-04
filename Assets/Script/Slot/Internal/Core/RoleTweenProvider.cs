using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Slot.Sequences.Core
{
    /// <summary>
    /// Roleを制御する各種Tweenを提供する
    /// </summary>
    public class RoleTweenProvider
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
        /// 正面とする角度
        /// </summary>
        private readonly Angle m_frontAngle;
        /// <summary>
        /// 各Role間の角度の間隔
        /// </summary>
        private readonly Angle m_roleIntervalAngle;
        /// <summary>
        /// 初期状態の座標
        /// </summary>
        private readonly Vector3 m_initialPosition;
        /// <summary>
        /// リールの半径
        /// </summary>
        private readonly float m_radius;
        /// <summary>
        /// 現在の角度
        /// </summary>
        private Angle m_nowAngle;

        /// <summary>
        /// このDriverが操るRoleの値
        /// </summary>
        public RoleValue RoleValue => m_operation.Value;
        /// <summary>
        /// Roleの識別子
        /// </summary>
        public IRole RoleID => m_operation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="initialAngle">初期状態の角度</param>
        /// <param name="radius">Reelの半径</param>
        public RoleTweenProvider(IRoleOperation operation, Angle initialAngle, float radius, Angle frontAngle, Angle roleInterval)
        {
            m_operation = operation;
            m_initialPosition = operation.transform.position;
            m_initialAngle = initialAngle;
            m_radius = radius;
            m_frontAngle = frontAngle;
            m_roleIntervalAngle = roleInterval;

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
        public Tween Roll(Angle angle, float duration, AngleTweenDirection direction = AngleTweenDirection.Both) =>
            RollAbsolutely(angle, duration, direction).SetRelative();

        /// <summary>
        /// Roleの間隔の倍数を指定により、現在の角度からReelを回転させる
        /// </summary>
        /// <param name="multiple">Roleの間隔の何倍の角度ぶん、回転させるか</param>
        /// <param name="duration"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Tween RollMultiple(float multiple, float duration, AngleTweenDirection direction = AngleTweenDirection.Both) =>
            Roll(m_roleIntervalAngle * multiple, duration, direction);
            
        /// <summary>
        /// 揃ったときの演出シーケンスを取得する
        /// </summary>
        /// <returns></returns>
        public Sequence GetWinningProductionSequence()
        {
            return DOTween.Sequence()
                          .Append(WinningRotate())
                          .Append(WinningZoomUp())
                          .AppendInterval(0.5f)
                          .Append(WinningZoomDown());

            Tween WinningRotate() =>
                m_operation.transform.DORotate(new Vector3(0, 720, 0), 0.5f).SetRelative();

            Tween WinningZoomUp() =>
                m_operation.transform.DOLocalMoveZ(-0.5f, 0.5f)
                                     .SetRelative();
            Tween WinningZoomDown() =>
                m_operation.transform.DOLocalMoveZ(0.5f, 0.5f)
                                     .SetRelative();
        }

        /// <summary>
        /// Transformを使った任意のTweenを生成します
        /// </summary>
        /// <param name="tweenSelector"></param>
        /// <returns></returns>
        public Tween Create(Func<Transform, Tween> tweenSelector)
        {
            return tweenSelector(m_operation.transform);
        }

        /// <summary>
        /// Transformを使った任意のSequenceを生成します
        /// </summary>
        /// <param name="sequenceSelector"></param>
        /// <returns></returns>
        public Sequence Create(Func<Transform, Sequence> sequenceSelector)
        {
            return sequenceSelector(m_operation.transform);
        }

        /// <summary>
        /// リールから外れたRoleをリール上に戻すTweenを取得します
        /// </summary>
        /// <param name="angle">戻す位置</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Tween BackToReelPosition(Angle angle, float duration) =>
            m_operation.transform.DOMove(OnReelPosition(angle), duration);


        /// <summary>
        /// 指定の角度にRoleを回転移動する
        /// </summary>
        /// <param name="targetAngle"></param>
        private void ApplyAngle(Angle targetAngle)
        {
            //Roleのpositionを回転させる
            m_operation.transform.position = OnReelPosition(targetAngle);
            //透明度を更新する
            m_operation.ChangeOpacity(getOpacity());
            //テーブルを更新
            m_nowAngle = targetAngle;

            float getOpacity()
            {
                //正面の角度からの差分の絶対値を算出
                var diffOfFront = (m_frontAngle - targetAngle.Normalize()).Absolute();

                //表示範囲より外側は不透明度0
                if (diffOfFront > RoleDisplayAngle) return 0;
                //内側なら外に行くたびに透明になる
                else return 1 - diffOfFront.TotalDegree / RoleDisplayAngle.TotalDegree;
            }
        }

        /// <summary>
        /// 指定角度のときのリール上の座標を取得します。
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Vector3 OnReelPosition(Angle angle) =>
           m_radius * angle.Point.AddX(0) + m_initialPosition;

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