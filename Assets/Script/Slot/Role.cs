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
    }
    public class Role : MonoBehaviour, IRoleOperation
    {
        [SerializeField]
        private RoleValue m_value;
        // Start is called before the first frame update
        void Start()
        {
        }

        public RoleValue Value => m_value;
    }
}