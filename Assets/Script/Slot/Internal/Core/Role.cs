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
    /// <summary>
    /// スロットの役
    /// </summary>
    public class Role : MonoBehaviour, IRoleOperation
    {
        [SerializeField]
        private RoleValue value;

        private Material material;

        void Awake()
        {
            material = GetComponentInChildren<Renderer>().material;
        }

        public void ChangeOpacity(float opacity)
        {
            material.color = new Color(material.color.r,
                                       material.color.g,
                                       material.color.b,
                                       opacity);
        }

        public RoleValue Value => value;
    }
}