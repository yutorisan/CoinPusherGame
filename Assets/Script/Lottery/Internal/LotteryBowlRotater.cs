using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台の回転状況の購読を提供する
    /// </summary>
    internal interface IObservableLotteryRotater
    {
        /// <summary>
        /// 抽選台の回転角度
        /// </summary>
        IObservable<float> ObservableRotate { get; }
    }
    /// <summary>
    /// 抽選台を回転させる
    /// </summary>
    internal class LotteryBowlRotater : MonoBehaviour, IObservableLotteryRotater
    {
        [SerializeField]
        private float m_rotateSpeed = 1f;
        [Inject]
        private IObservableBallCount onBallCount;

        private Rigidbody _rigidbody;
        private ReactiveProperty<float> _rotateAngle = new ReactiveProperty<float>(0f);

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            //抽選台の上にボールが1個以上存在したら回転させる
            if(onBallCount.BallCount.Value > 0)
            {
                _rigidbody.MoveRotation(Quaternion.Euler(0, _rotateAngle.Value += m_rotateSpeed, 0));
            }
        }

        public IObservable<float> ObservableRotate => _rotateAngle.AsObservable();

    }
}