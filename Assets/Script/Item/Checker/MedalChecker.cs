using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// コライダーによってメダルの通過を検知して、検出通知を外部に公開する
    /// </summary>
    public class MedalChecker : MonoBehaviour, IObservableInColliderCountableMedalChecker, IObservableMedalChecker
    {
        /// <summary>
        /// メダルを検出したときに同時にDestroyするか
        /// </summary>
        [SerializeField]
        private bool isDestroyOnChecked;
        /// <summary>
        /// 
        /// </summary>
        private readonly ReactiveProperty<int> stayInCount = new ReactiveProperty<int>();

        // Use this for initialization
        void Awake()
        {
            //TriggerしたもののtagがMedalだったら、そのIMedalコンポーネントを発行する
            Checked = this.OnTriggerEnterAsObservable()
                          .Where(col => col.CompareTag("Medal"))
                          .Select(col => col.GetComponent<IMedal>())
                          .Share();
            //IsDestroyOnCheckedにチェックがはいっていたら、Checkedでプールに戻す
            if (isDestroyOnChecked)
                Checked.Subscribe(medal => medal.ReturnToPool());

            //EnterとExitの数を数えてCollider内にいるメダル数をカウント
            var enter = this.OnTriggerEnterAsObservable()
                            .Where(col => col.CompareTag("Medal"))
                            .Select(col => col.GetComponent<IMedal>());
            var exit = this.OnTriggerExitAsObservable()
                           .Where(col => col.CompareTag("Medal"))
                           .Select(col => col.GetComponent<IMedal>());
            enter.Subscribe(_ => ++stayInCount.Value);
            exit.Subscribe(_ => --stayInCount.Value);
        }

        public IObservable<IMedal> Checked { get; private set; }
        public IObservable<int> InColliderCount => stayInCount.AsObservable();
    }
}