using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// コライダーによってメダルの通過を検知して、検出通知を外部に公開する
    /// </summary>
    public class MedalChecker : MonoBehaviour, IObservableMedalChecker
    {
        /// <summary>
        /// メダルを検出したときに同時にDestroyするか
        /// </summary>
        [SerializeField]
        private bool isDestroyOnChecked;

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
        }

        public IObservable<IMedal> Checked { get; private set; }
    }
}