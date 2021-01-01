using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    public class MedalChecker : MonoBehaviour, IObservableMedalChecker
    {
        [SerializeField]
        private bool m_isDestroyOnChecked;

        private IObservable<IMedal> _triggertrigger;

        // Use this for initialization
        void Awake()
        {
            //TriggerしたもののtagがMedalだったら、そのIMedalコンポーネントを発行する
            Checked = this.OnTriggerEnterAsObservable()
                          .Where(col => col.CompareTag("Medal"))
                          .Select(col => col.GetComponent<IMedal>())
                          .Share();


            if (m_isDestroyOnChecked)
                Checked.Subscribe(medal => medal.ReturnToPool());
        }

        public IObservable<IMedal> Checked { get; private set; }

    }
}