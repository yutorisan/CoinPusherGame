﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace MedalPusher
{
    /// <summary>
    /// プッシャーを動かす
    /// </summary>
    public class PusherMover : MonoBehaviour
    {
        /// <summary>
        /// 移動周期
        /// </summary>
        [SerializeField]
        private float m_cycleSecond = 6f;
        /// <summary>
        /// 移動する長さ
        /// </summary>
        [SerializeField]
        private float m_moveLength = 0.05f;

        /// <summary>
        /// 初期位置
        /// </summary>
        private Vector3 m_initPos;
        private Rigidbody m_rigidbody;

        void Start()
        {
            m_initPos = transform.position;
            m_rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Angle angle = Angle.FromDegree(360 * (Time.time % m_cycleSecond) / m_cycleSecond);
            var diff = m_moveLength * Mathf.Sin(angle.TotalRadian);
            m_rigidbody.MovePosition(m_initPos.PlusZ(diff));
        }
    }
}