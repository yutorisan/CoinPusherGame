using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台を回転させる
    /// </summary>
    internal class LotteryBowlRotater : MonoBehaviour
    {
        [SerializeField]
        private float m_rotateSpeed = 1f;
        [Inject]
        private IObservableBallCount onBallCount;

        private Rigidbody _rigidbody;
        private float _rotateAngle;

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
            }
        }
    }
}