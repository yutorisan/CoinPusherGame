using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using MedalPusher.Item;
using Zenject;

namespace MedalPusher.Lottery {
    public class LotteryPocket : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private TextMesh _priseView;
        private ParticleSystem _particle;

        [SerializeField]
        private LotteryPrizeInfo m_prizeInfo;

        [Inject]
        private ILotteryPrizeInsertionSlot _prizeInsertionSlot;
        //[Inject]
        //private IObservableLotteryRotater _observableLotteryRotater;

        // Start is called before the first frame update
        void Start()
        {
            //子オブジェクトのコンポーネントを取得する
            _priseView = GetComponentInChildren<TextMesh>();
            _particle = GetComponentInChildren<ParticleSystem>();

            //自分のポケットにボールが入ったら、自身の持つ景品を投入する
            this.OnTriggerEnterAsObservable()
                .Where(col => col.CompareTag("LotteryBall"))
                .Subscribe(_ =>
                {
                    _prizeInsertionSlot.InsertPrize(m_prizeInfo);
                    _particle.Play();
                });
        }

        private void OnValidate()
        {
            _priseView = GetComponentInChildren<TextMesh>();
            _priseView.text = m_prizeInfo.PrizeMedals.ToString();
        }


    }
}