using System;
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
    }
    public class Medal : MonoBehaviour, IMedal
    {
        [SerializeField]
        private MedalValue m_value;

        public int Value => (int)m_value;
        public MedalValue ValueType => m_value;

        public event Action<MedalValue> OnReturnToPool;

        public void ReturnToPool()
        {
            OnReturnToPool(m_value);
            this.gameObject.SetActive(false);
        }
    }

    public enum MedalValue
    {
        Value1 = 1,
        Value3 = 3,
        Value10 = 10,
    }
}