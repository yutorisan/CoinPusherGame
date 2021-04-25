using System;
using System.Security;
using MedalPusher.Item.Pool;
using UniRx;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Pool
{
    /// <summary>
    /// このオブジェクトの使用状況を参照可能
    /// </summary>
    public interface IObservablePoolObjectStatus
    {
        /// <summary>
        /// PoolObjectの使用状況を取得・購読します。
        /// </summary>
        IReadOnlyReactiveProperty<PoolObjectUseStatus> Status { get; }
    }
    /// <summary>
    /// このオブジェクトを使用できるようにマーク可能
    /// </summary>
    public interface IPoolObjectKeeper : IObservablePoolObjectStatus
    {
        /// <summary>
        /// このオブジェクトの出現位置を指定して、このオブジェクトを使用中とマークする
        /// </summary>
        void Keep(Vector3 position);
        /// <summary>
        /// このオブジェクトの出現位置と回転を指定して、このオブジェクトを使用中とマークする
        /// </summary>
        void Keep(Vector3 position, Quaternion rotation);
    }
    /// <summary>
    /// このオブジェクトを使用しなくなったとマーク可能
    /// </summary>
    public interface IPoolObjectReleaser : IObservablePoolObjectStatus
    {
        /// <summary>
        /// このオブジェクトを使用していないとマークする
        /// </summary>
        void Release();
    }
    /// <summary>
    /// オブジェクトプールで管理されるオブジェクト
    /// 初期化時のactive状態はfalse
    /// </summary>
    public abstract class PoolObject : MonoBehaviour, IPoolObjectKeeper, IPoolObjectReleaser
    {
        private IUseStatus useStatus;

        private void Awake()
        {
            //PoolObjectは生成時は自動でfalse
            this.gameObject.SetActive(false);

            useStatus = new UseStatus(this.gameObject);
        }

        /// <summary>
        /// プールオブジェクトをプールから取り出して使用可能にする
        /// rotationはQuaternion.identity
        /// </summary>
        /// <param name="position"></param>
        public void Keep(Vector3 position) => Keep(position, Quaternion.identity);
        /// <summary>
        /// プールオブジェクトをプールから取り出して使用可能にする
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void Keep(Vector3 position, Quaternion rotation)
        {
            this.gameObject.transform.position = position;
            this.gameObject.transform.rotation = rotation;
            useStatus.ChangeUseStatus(PoolObjectUseStatus.Used);
        }

        public void Release()
        {
            useStatus.ChangeUseStatus(PoolObjectUseStatus.Idol);
        }

        public IReadOnlyReactiveProperty<PoolObjectUseStatus> Status => useStatus.Status;

        /// <summary>
        /// 使用状況オブジェクトへのアクセス
        /// </summary>
        private interface IUseStatus
        {
            /// <summary>
            /// 使用状況を変更する
            /// </summary>
            /// <param name="isUsed"></param>
            /// <exception cref="PoolObjectInvalidOperationException">同一のステータスをセットしようとしたときにスローされます。</exception>
            void ChangeUseStatus(PoolObjectUseStatus isUsed);
            /// <summary>
            /// 使用状況を取得・購読する
            /// </summary>
            IReadOnlyReactiveProperty<PoolObjectUseStatus> Status { get; }
        }
        /// <summary>
        /// このGameObjectの使用状況をカプセル化
        /// </summary>
        private class UseStatus : IUseStatus
        {
            /// <summary>
            /// 対象のGameObject
            /// </summary>
            private GameObject parentGObj;
            /// <summary>
            /// このGameObjectの使用状況
            /// </summary>
            private readonly ReactiveProperty<PoolObjectUseStatus> useStatus = new ReactiveProperty<PoolObjectUseStatus>(PoolObjectUseStatus.Idol);

            public UseStatus(GameObject parent)
            {
                parentGObj = parent;
            }

            public void ChangeUseStatus(PoolObjectUseStatus newUseStatus)
            {
                //使用状況を同一の値で上書きしようとした場合は例外発生させる
                if (useStatus.Value == newUseStatus) throw new PoolObjectInvalidOperationException();
                parentGObj.SetActive(newUseStatus == PoolObjectUseStatus.Used);
                useStatus.Value = newUseStatus;
            }

            public IReadOnlyReactiveProperty<PoolObjectUseStatus> Status => useStatus;
        }
    }

    /// <summary>
    /// PoolObjectの使用状況
    /// </summary>
    public enum PoolObjectUseStatus
    {
        /// <summary>
        /// 使用されていない
        /// </summary>
        Idol,
        /// <summary>
        /// 使用されている
        /// </summary>
        Used
    }

    /// <summary>
    /// プールオブジェクトへのアクセス異常例外
    /// </summary>
    public class PoolObjectInvalidOperationException : Exception
    {
        public PoolObjectInvalidOperationException() : base() { }
        public PoolObjectInvalidOperationException(string message) : base(message) { }
    }
}