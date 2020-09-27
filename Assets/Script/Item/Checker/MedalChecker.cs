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
            ItemChecked = this.OnTriggerEnterAsObservable()
                              .Where(col => col.tag == "Medal")
                              .Select(col => col.GetComponent<IMedal>())
                              .Share();


            if (m_isDestroyOnChecked)
                ItemChecked.Subscribe(m => m.ReturnToPool());
        }

        public IObservable<IMedal> ItemChecked { get; private set; }

    }
}