using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    public class MedalChecker : MonoBehaviour, IObservableItemChecker<IMedal>
    {
        [SerializeField]
        private bool m_isDestroyOnChecked;

        private IObservable<IMedal> _triggertrigger;

        // Use this for initialization
        void Awake()
        {
            //TriggerしたもののtagがMedalだったら、そのIMedalコンポーネントを発行する
            var triggerMedal = this.OnTriggerEnterAsObservable()
                                   .Where(col => col.tag == "Medal")
                                   .Share();
            ItemChecked = triggerMedal.Select(col => col.GetComponent<IMedal>());


            if (m_isDestroyOnChecked)
                triggerMedal.Subscribe(m => Destroy(m));
        }

        public IObservable<IMedal> ItemChecked { get; private set; }

    }
}