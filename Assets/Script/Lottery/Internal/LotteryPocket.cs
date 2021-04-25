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
        private TextMesh priseView;
        private ParticleSystem particle;

        [SerializeField]
        private LotteryPrizeInfo prizeInfo;
        [SerializeField]
        private int fontSize = 80;

        [Inject]
        private ILotteryPrizeInsertionSlot prizeInsertionSlot;

        protected override string DetectTag => "LotteryBall";

        void Start()
        {
            //子オブジェクトのコンポーネントを取得する
            priseView = GetComponentInChildren<TextMesh>();
            particle = GetComponentInChildren<ParticleSystem>();

            //自分のポケットにボールが入ったら、自身の持つ景品を投入する
            this.Checked.Subscribe(_ =>
            {
                prizeInsertionSlot.InsertPrize(prizeInfo);
                particle.Play();
            }).AddTo(this);
        }

        private void OnValidate()
        {
            priseView = GetComponentInChildren<TextMesh>();
            priseView.text = prizeInfo.PrizeMedals.ToString();
            priseView.fontSize = fontSize;
        }


    }
}