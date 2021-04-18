using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Zenject;
using MedalPusher.Item.Checker;

namespace MedalPusher.Lottery
{
    public class LotteryPocket : CheckerBase<LotteryBall>
    {
        [SerializeField, HideInInspector]
        private TextMesh _priseView;
        private ParticleSystem _particle;

        [SerializeField]
        private LotteryPrizeInfo m_prizeInfo;
        [SerializeField]
        private int m_fontSize = 80;

        [Inject]
        private ILotteryPrizeInsertionSlot _prizeInsertionSlot;

        protected override string DetectTag => "LotteryBall";

        void Start()
        {
            //子オブジェクトのコンポーネントを取得する
            _priseView = GetComponentInChildren<TextMesh>();
            _particle = GetComponentInChildren<ParticleSystem>();

            //自分のポケットにボールが入ったら、自身の持つ景品を投入する
            this.Checked.Subscribe(_ =>
            {
                _prizeInsertionSlot.InsertPrize(m_prizeInfo);
                _particle.Play();
            }).AddTo(this);
        }

        private void OnValidate()
        {
            _priseView = GetComponentInChildren<TextMesh>();
            _priseView.text = m_prizeInfo.PrizeMedals.ToString();
            _priseView.fontSize = m_fontSize;
        }


    }
}