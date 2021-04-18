using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Slot.Internal.Core
{
    /// <summary>
    /// RoleのGameObjectを操作可能
    /// </summary>
    public interface IRoleOperation
    {
        /// <summary>
        /// Transformオブジェクトを取得する
        /// </summary>
        Transform transform { get; }
        /// <summary>
        /// このRoleの値を取得する
        /// </summary>
        RoleValue Value { get; }
        /// <summary>
        /// RoleのMaterialの透明度を設定する
        /// </summary>
        /// <param name="opacity"></param>
        void ChangeOpacity(float opacity);
    }
    public class Role : MonoBehaviour, IRoleOperation
    {
        [SerializeField]
        private RoleValue m_value;

        private Material m_material;

        void Awake()
        {
            m_material = GetComponentInChildren<Renderer>().material;
        }

        public void ChangeOpacity(float opacity)
        {
            m_material.color = new Color(m_material.color.r,
                                         m_material.color.g,
                                         m_material.color.b,
                                         opacity);
        }

        public RoleValue Value => m_value;
    }
}