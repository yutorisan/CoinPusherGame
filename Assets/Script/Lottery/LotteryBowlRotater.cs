using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// 抽選台の回転状況の購読を提供する
    /// </summary>
    public interface IObservableLotteryRotater
    {
        /// <summary>
        /// 抽選台の回転角度
        /// </summary>
        IObservable<float> ObservableRotate { get; }
    }

    public class LotteryBowlRotater : MonoBehaviour, IObservableLotteryRotater
    {
        [SerializeField]
        private float m_rotateSpeed = 1f;

        private Rigidbody _rigidbody;
        private ReactiveProperty<float> _rotateAngle = new ReactiveProperty<float>(0f);

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            _rigidbody.MoveRotation(Quaternion.Euler(0, _rotateAngle.Value += m_rotateSpeed, 0));
        }

        public IObservable<float> ObservableRotate => _rotateAngle.AsObservable();

    }
}