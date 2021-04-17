using System;
using MedalPusher.Item.Pool;
using UnityEngine;

namespace MedalPusher.Item
{
    public interface IMedal : IFieldObject, IPoolObject
    {
        /// <summary>
        /// メダルの価値（枚数）
        /// </summary>
        int Value { get; }
        /// <summary>
        /// メダル価値の種類
        /// </summary>
        MedalValue ValueType { get; }
        /// <summary>
        /// Positionを取得する
        /// </summary>
        Vector3 position { get; }
    }
    public class Medal : PoolObject, IMedal
    {
        [SerializeField]
        private MedalValue m_value;

        public int Value => (int)m_value;
        public MedalValue ValueType => m_value;
        public Vector3 position => transform.position;

        public void Dispose() => ReturnToPool();
    }

    /// <summary>
    /// メダルの種別（メダルの価値）
    /// </summary>
    public enum MedalValue
    {
        Value1 = 1,
        Value3 = 3,
        Value10 = 10,
    }
}