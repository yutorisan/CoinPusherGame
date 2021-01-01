using System;
using System.Security;
using MedalPusher.Item.Pool;
using UniRx;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Pool
{
    /// <summary>
    /// 読み取りのみ可能なPoolObjectのインターフェイス
    /// </summary>
    public interface IReadOnlyPoolObject
    {
        /// <summary>
        /// 使用状況
        /// </summary>
        IReadOnlyReactiveProperty<PoolObjectUseStatus> Status { get; }
    }
    /// <summary>
    /// オブジェクトプールへ返却のみ可能なPoolObjectのインターフェイス
    /// </summary>
    public interface IReturnOnlyPoolObject : IReadOnlyPoolObject
    {
        /// <summary>
        /// オブジェクトをプールに返す
        /// </summary>
        void ReturnToPool();
    }
    /// <summary>
    /// オブジェクトプールで管理されるオブジェクトであることを示す
    /// </summary>
    public interface IPoolObject : IReturnOnlyPoolObject
    {
        /// <summary>
        /// プールオブジェクトをプールから取り出して使用可能にする
        /// rotationはQuaternion.identity
        /// </summary>
        /// <param name="position"></param>
        void Takeout(Vector3 position);
        /// <summary>
        /// プールオブジェクトをプールから取り出して使用可能にする
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        void Takeout(Vector3 position, Quaternion rotation);
    }

    /// <summary>
    /// オブジェクトプールで管理されるオブジェクト
    /// 初期化時のactive状態はfalse
    /// </summary>
    public abstract class PoolObject : MonoBehaviour, IPoolObject
    {
        private readonly UniReadOnly<IUseStatus> useStatus = new UniReadOnly<IUseStatus>();

        private void Awake()
        {
            //PoolObjectは生成時は自動でfalse
            this.gameObject.SetActive(false);

            useStatus.Initialize(new UseStatus(this.gameObject));
        }

        public void Takeout(Vector3 position) => Takeout(position, Quaternion.identity);
        public void Takeout(Vector3 position, Quaternion rotation)
        {
            this.gameObject.transform.position = position;
            this.gameObject.transform.rotation = rotation;
            useStatus.Value.ChangeUseStatus(PoolObjectUseStatus.Used);
        }

        public void ReturnToPool()
        {
            useStatus.Value.ChangeUseStatus(PoolObjectUseStatus.Idol);
        }

        public IReadOnlyReactiveProperty<PoolObjectUseStatus> Status => useStatus.Value.Status;


        private interface IUseStatus
        {
            /// <summary>
            /// 使用状況を変更する
            /// </summary>
            /// <param name="isUsed"></param>
            void ChangeUseStatus(PoolObjectUseStatus isUsed);
            /// <summary>
            /// 使用状況を取得・購読する
            /// </summary>
            IReadOnlyReactiveProperty<PoolObjectUseStatus> Status { get; }
        }
        private class UseStatus : IUseStatus
        {
            private bool _isUsed;
            private GameObject _parentGObj;
            private readonly ReactiveProperty<PoolObjectUseStatus> _useStatus = new ReactiveProperty<PoolObjectUseStatus>(PoolObjectUseStatus.Idol);

            public UseStatus(GameObject parent)
            {
                _parentGObj = parent;
            }

            public void ChangeUseStatus(PoolObjectUseStatus newUseStatus)
            {
                //使用状況を同一の値で上書きしようとした場合は例外発生させる
                if (_useStatus.Value == newUseStatus) throw new PoolObjectInvalidOperationException();
                _parentGObj.SetActive(newUseStatus.IsUsed());
                _useStatus.Value = newUseStatus;
            }

            public IReadOnlyReactiveProperty<PoolObjectUseStatus> Status => _useStatus;
        }
    }

    public enum PoolObjectUseStatus
    {
        Idol,
        Used
    }

    /// <summary>
    /// 使用中のプールオブジェクトを重複して使用しようとしたり、未使用のプールオブジェクトを返そうとした場合に発生
    /// </summary>
    public class PoolObjectInvalidOperationException : Exception
    {
        public PoolObjectInvalidOperationException() : base() { }
        public PoolObjectInvalidOperationException(string message) : base(message) { }
    }
}

public static class Ex
{
    public static bool IsUsed(this PoolObjectUseStatus status) => status == PoolObjectUseStatus.Used;
}