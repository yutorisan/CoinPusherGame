using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityUtility;
using UnityUtility.Extensions;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// 各リールの動きを制御することができる
    /// </summary>
    public interface IReelDriver
    {
        /// <summary>
        /// 指定の役を
        /// </summary>
        /// <param name="role"></param>
        /// <param name="rollTimes"></param>
        void BringToFront(RoleValue role, int rollTimes);

    }
    /// <summary>
    /// 各リールの動きを制御する
    /// </summary>
    public class ReelDriver : SerializedMonoBehaviour, IReelDriver
    {
        [SerializeField]
        private IReelOperation m_reelOperation;

        /// <summary>
        /// スロットの回転半径
        /// </summary>
        [SerializeField]
        private float m_radius;

        private UniReadOnly<RoleDegreeTable> _roleDegreeTable = new UniReadOnly<RoleDegreeTable>();

        // Start is called before the first frame update
        void Start()
        {
            var roleCnt = m_reelOperation.Roles.Count;
            _roleDegreeTable.Initialize(new RoleDegreeTable(
                m_reelOperation.Roles
                               .Select((ope, index) => new { ope, index })
                               .ToDictionary(pair => pair.ope,
                                             pair => 360f / roleCnt * pair.index)));
        }

        public void BringToFront(RoleValue roleVal, int rollTimes)
        {

        }

        [Button("Roll")]
        public void Roll(float degree)
        {
            foreach (var role in m_reelOperation.Roles)
            {
                DOTween.To(() => 0,
                    newDeg =>
                    {
                        RotateRole(role, newDeg);
                    },
                    _roleDegreeTable.Value[role] + degree, 0.5f);
            }
        }

        private float GetNewRadian(int index) => 2 * Mathf.PI / m_reelOperation.Roles.Count * index;


        private void OnValidate()
        {
            int objCount = m_reelOperation.Roles.Count;

            m_reelOperation.Roles
                .Select(ope => ope.transform)
                .Select((transform, i) => new
                {
                    transform,
                    transform.position,
                    z = m_radius * Mathf.Cos(2 * Mathf.PI / objCount * i),
                    y = m_radius * Mathf.Sin(2 * Mathf.PI / objCount * i)
                })
                .ForEach(pair => pair.transform.position = new Vector3(pair.position.x, pair.y, pair.z));
        }

        /// <summary>
        /// 初期状態を基準として、指定の角度に回転させる
        /// </summary>
        /// <param name="targetDegree"></param>
        private void RotateRole(IRoleOperation role, float targetDegree)
        {
            //Roleのpositionを回転させる
            role.transform.position = new Vector3(
                role.transform.position.x,
                m_radius * Mathf.Cos(targetDegree.ToRadian()),
                m_radius * Mathf.Sin(targetDegree.ToRadian()));
            //テーブルを更新
            _roleDegreeTable.Value[role] = targetDegree;
        }

        private class RoleDegreeTable
        {
            private readonly IReadOnlyDictionary<IRoleOperation, float> m_initialTable;
            private readonly Dictionary<IRoleOperation, float> m_operationTable;
            private readonly Dictionary<RoleValue, float> m_roleValueTable;

            public RoleDegreeTable(IReadOnlyDictionary<IRoleOperation, float> initialTable)
            {
                m_initialTable = initialTable;
                m_operationTable = initialTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                m_roleValueTable = initialTable.ToDictionary(kvp => kvp.Key.Value, kvp => kvp.Value);
            }

            public float this[IRoleOperation index]
            {
                get => m_operationTable[index];
                set => m_operationTable[index] = CorrectDegreeRange(value);
            }
            public float this[RoleValue index]
            {
                get => m_roleValueTable[index];
                set => m_roleValueTable[index] = CorrectDegreeRange(value);
            }

            /// <summary>
            /// 角度を0~360の範囲に修正する
            /// </summary>
            /// <param name="degree"></param>
            /// <returns></returns>
            private float CorrectDegreeRange(float degree)
            {
                if (degree > 360) return CorrectDegreeRange(degree - 360);
                if (degree < 0) return CorrectDegreeRange(degree + 360);
                return degree;
            }
        }
    }
}