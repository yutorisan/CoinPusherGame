using System;
using UnityEngine;

namespace MedalPusher.Item
{

    public interface IFieldItem : IFieldObject
    {
        /// <summary>
        /// アイテムを使用する
        /// </summary>
        void Use();
    }
    /// <summary>
    /// フィールド上に出現するアイテム
    /// </summary>
    public abstract class FieldItem : MonoBehaviour, IFieldItem
    {
        protected abstract IGameEvent GameEvent { get; }

        public void Dispose() => Destroy(gameObject);

        public void Use() => GameEvent.Occur();
    }
}