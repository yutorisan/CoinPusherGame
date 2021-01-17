using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Slot
{
    public interface IRoleOperation
    {
        Transform transform { get; }
        RoleValue Value { get; }
        void ChangeOpacity(float opacity);
    }
    public class Role : MonoBehaviour, IRoleOperation
    {
        [SerializeField]
        private RoleValue m_value;

        private Material m_material;
        // Start is called before the first frame update
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