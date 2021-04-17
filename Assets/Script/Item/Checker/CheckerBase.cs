using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IObservableItemChecker<out TItem> where TItem : IFieldObject
    {
        /// <summary>
        /// アイテムを検出した
        /// </summary>
        IObservable<TItem> Checked { get; }
    }
    /// <summary>
    /// メダルの検出通知を受け取る
    /// </summary>
    public interface IObservableMedalChecker : IObservableItemChecker<IMedal> { }
    /// <summary>
    /// アイテムの検出通知を受け取る
    /// </summary>
    public interface IObservableFieldItemChecker : IObservableItemChecker<IFieldItem> { }

    /// <summary>
    /// コライダーによって<see cref="IFieldObject"/>の通過を検知して、検出通知を外部に公開する
    /// </summary>
    /// <typeparam name="T"><see cref="IFieldObject"/>を実装した検出対処アイテム</typeparam>
    public abstract class CheckerBase<T> : MonoBehaviour, IObservableItemChecker<T> where T : IFieldObject
    {
        /// <summary>
        /// メダルを検出したときに同時にDestroyするか
        /// </summary>
        [SerializeField]
        protected bool isDisposeOnChecked;

        void Awake()
        {
            //TriggerしたもののtagがMedalだったら、そのIMedalコンポーネントを発行する
            Checked = this.OnTriggerEnterAsObservable()
                          .Where(col => col.CompareTag(DetectTag))
                          .Select(col => col.GetComponent<T>())
                          .Share();
            //IsDisposeOnCheckedにチェックがはいっていたら、Checkedでオブジェクトを破棄
            if (isDisposeOnChecked)
                Checked.Subscribe(fieldObject => fieldObject.Dispose());
        }

        /// <summary>
        /// Checkerの検出対象とするタグ名
        /// </summary>
        protected abstract string DetectTag { get; }

        public IObservable<T> Checked { get; private set; }

    }
}
