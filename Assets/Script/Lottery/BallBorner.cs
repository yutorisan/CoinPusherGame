using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MedalPusher.Lottery
{
    /// <summary>
    /// ボールの生成を指示する
    /// </summary>
    public interface IBallBornOperator
    {
        /// <summary>
        /// ボールを生成する
        /// </summary>
        void Born();
    }
    /// <summary>
    /// ボールを生成する
    /// </summary>
    public class BallBorner : MonoBehaviour, IBallBornOperator
    {
        [SerializeField]
        private UnityEngine.Object m_ballPrefab;

        public void Born()
        {
            Instantiate(m_ballPrefab, this.transform.position, Quaternion.identity);
        }
    }
}