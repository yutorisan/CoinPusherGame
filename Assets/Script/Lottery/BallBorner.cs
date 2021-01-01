using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MedalPusher.Lottery
{
    public class BallBorner : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Object m_ballPrefab;
        [SerializeField]
        private float m_bornIntervalSecond;

        // Start is called before the first frame update
        void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(m_bornIntervalSecond))
                      .Subscribe(_ => Instantiate(m_ballPrefab, this.transform.position, Quaternion.identity));
        }

    }
}