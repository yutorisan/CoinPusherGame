using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台の状況を取得・購読可能
    /// </summary>
    public interface IObservableLotteryStatus
    {
        IReadOnlyReactiveProperty<LotteryStatus> Status { get; }
    }
    /// <summary>
    /// 抽選台を回転させる
    /// </summary>
    public class LotteryBowlRotater : MonoBehaviour, IObservableLotteryStatus
    {
        [SerializeField]
        private float m_rotateSpeed = 1f;
        [Inject]
        private IObservableBallCount onBallCount;

        private ReactiveProperty<LotteryStatus> status = new ReactiveProperty<LotteryStatus>();
        private Rigidbody _rigidbody;
        private float _rotateAngle;

        public IReadOnlyReactiveProperty<LotteryStatus> Status => status;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            //抽選台の上にボールが1個以上存在したら回転させる
            if(onBallCount.BallCount.Value > 0)
            {
                _rigidbody.MoveRotation(Quaternion.Euler(0, _rotateAngle += m_rotateSpeed * Time.deltaTime, 0));
                status.Value = LotteryStatus.Lotterying;
            }
            else
            {
                status.Value = LotteryStatus.Idol;
            }
        }
    }
    public enum LotteryStatus
    {
        Idol,
        Lotterying
    }
}