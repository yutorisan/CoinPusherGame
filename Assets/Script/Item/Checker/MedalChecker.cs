using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    public class MedalChecker : MonoBehaviour, IObservableInColliderCountableMedalChecker, IObservableMedalChecker
    {
        [SerializeField]
        private bool m_isDestroyOnChecked;

        private IObservable<IMedal> _triggertrigger;
        private ReactiveProperty<int> _stayInCount = new ReactiveProperty<int>();

        // Use this for initialization
        void Awake()
        {
            //TriggerしたもののtagがMedalだったら、そのIMedalコンポーネントを発行する
            Checked = this.OnTriggerEnterAsObservable()
                          .Where(col => col.CompareTag("Medal"))
                          .Select(col => col.GetComponent<IMedal>())
                          .Share();
            //IsDestroyOnCheckedにチェックがはいっていたら、Checkedでプールに戻す
            if (m_isDestroyOnChecked)
                Checked.Subscribe(medal => medal.ReturnToPool());

            //EnterとExitの数を数えてCollider内にいるメダル数をカウント
            var enter = this.OnTriggerEnterAsObservable()
                            .Where(col => col.CompareTag("Medal"))
                            .Select(col => col.GetComponent<IMedal>());
            var exit = this.OnTriggerExitAsObservable()
                           .Where(col => col.CompareTag("Medal"))
                           .Select(col => col.GetComponent<IMedal>());
            enter.Subscribe(_ => ++_stayInCount.Value);
            exit.Subscribe(_ => --_stayInCount.Value);
        }

        public IObservable<IMedal> Checked { get; private set; }
        public IObservable<int> InColliderCount => _stayInCount.AsObservable();
    }
}