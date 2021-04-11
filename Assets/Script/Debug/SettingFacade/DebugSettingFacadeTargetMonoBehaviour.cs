using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MedalPusher.Debug
{
    /// <summary>
    /// デバッグ用
    /// DebugSettingFacadeによる一括設定の対象であることを示す
    /// </summary>
    public interface IDebugSettingFacadeTarget
    {
        /// <summary>
        /// 一括設定が完了したことを通知する
        /// </summary>
        void Already();
    }

    /// <summary>
    /// DebugSettingFacadeによる一括設定の対象となるMonoBehaviour
    /// </summary>
    public abstract class DebugSettingFacadeTargetMonoBehaviour : MonoBehaviour, IDebugSettingFacadeTarget
    {
        private readonly UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();

        public void Already()
        {
            uniTaskCompletionSource.TrySetResult();
        }

        /// <summary>
        /// DebugSettingFacadeによる一括設定が完了するまでのタスク
        /// このタスクが完了してから、それぞれの初期設定を行うようにする
        /// </summary>
        protected UniTask FacadeAlreadyOKAsync => uniTaskCompletionSource.Task;
    }
}