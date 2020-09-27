using System;
using UnityEngine;

namespace MedalPusher.Item
{
    public interface IReadOnlyMedal : IFieldObject
    {
        /// <summary>
        /// メダルの価値（枚数）
        /// </summary>
        int Value { get; }
    }
    public interface IMedal : IReadOnlyMedal
    {

    }
    public class Medal : MonoBehaviour, IMedal
    {
        [SerializeField]
        private int m_value;

        public int Value => m_value;
    }
}