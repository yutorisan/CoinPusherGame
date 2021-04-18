using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// ボールの生成を要求できる
    /// </summary>
    public interface IBallBornOperator
    {
        /// <summary>
        /// ボールを生成を要求する
        /// </summary>
        void BornRequest();
    }
    /// <summary>
    /// ボール生成通知を受け取れる
    /// </summary>
    internal interface IObservableBallBorned
    {
        /// <summary>
        /// ボールが生成されたときに発行されるイベント
        /// </summary>
        IObservable<Unit> BallBorned { get; }
    }
    /// <summary>
    /// ボールを生成する
    /// </summary>
    public class BallBorner : MonoBehaviour, IBallBornOperator, IObservableBallBorned
    {
        [SerializeField]
        private UnityEngine.Object m_ballPrefab;

        private readonly Subject<Unit> ballBornedSubject = new Subject<Unit>();

        public IObservable<Unit> BallBorned => ballBornedSubject.AsObservable();

        [Button]
        public void BornRequest()
        {
            Instantiate(m_ballPrefab, this.transform.position, Quaternion.identity);
            ballBornedSubject.OnNext(Unit.Default);
        }
    }
}   