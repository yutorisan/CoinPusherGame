using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.MPE;
using UnityEditor;
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
        ///// <summary>
        ///// 指定の役を
        ///// </summary>
        ///// <param name="role"></param>
        ///// <param name="rollTimes"></param>
        //void BringToFront(RoleValue role, int rollTimes);

    }
    /// <summary>
    /// 各リールの動きを制御する
    /// </summary>
    public class ReelDriver : SerializedMonoBehaviour, IReelDriver
    {
        /// <summary>
        /// Reelの初期状態から、0を全面に持ってくるまでの補正角度
        /// </summary>
        private static readonly float BringToFront0CorrectDegreeAngle = -60f;

        [SerializeField]
        private IReelOperation m_reelOperation;
        /// <summary>
        /// スロットの回転半径
        /// </summary>
        [SerializeField]
        private float m_radius;
        /// <summary>
        /// 初期状態からの回転角度
        /// </summary>
        [SerializeField, Range(-360, 360), OnValueChanged(nameof(OnDegreeChanged))]
        private float m_degree;

        /// <summary>
        /// Roleと回転角度に関する情報
        /// </summary>
        private UniReadOnly<RoleDegreeTable> _roleDegreeTable = new UniReadOnly<RoleDegreeTable>();
        /// <summary>
        /// Roleの初期座標
        /// </summary>
        private UniReadOnly<RoleInfoTable<Vector3>> _roleInitialPositionTable = new UniReadOnly<RoleInfoTable<Vector3>>();

        // Start is called before the first frame update
        void Start()
        {
            //すべてのRoleを円形に配置する
            _roleDegreeTable.Initialize(new RoleDegreeTable(
                m_reelOperation.Roles
                               .Select((ope, index) => new { ope, index })
                               .ToDictionary(pair => pair.ope,
                                             pair => 360f / m_reelOperation.Roles.Count * pair.index)));
            //すべてのRoleの初期座標を記憶する
            _roleInitialPositionTable.Initialize(new RoleInfoTable<Vector3>(
                m_reelOperation.Roles.ToDictionary(role => role, role => role.transform.position)));

            OnDegreeChanged();
        }

        //[Button("Bring")]
        //public void BringToFront(RoleValue roleVal, float duration)
        //{
        //    RollAbsolutely(BringToFront0CorrectDegreeAngle - 360 / m_reelOperation.Roles.Count * GetRoleIndex(roleVal), duration);
        //}

        /// <summary>
        /// 現在の角度から、任意の角度ぶんReelを回転させる
        /// </summary>
        /// <param name="degree">何度回転させるか</param>
        /// <param name="duration"></param>
        [Button("Roll")]
        public void RollRelatively(float degree, float duration)
        {
            foreach (var role in m_reelOperation.Roles)
            {
                DOTween.To(() => _roleDegreeTable.Value[role],
                    newDeg =>
                    {
                        ApplyAngle(role, newDeg);
                    },
                    _roleDegreeTable.Value[role] + degree, duration);
            }
        }
        /// <summary>
        /// Reelを指定の角度まで回転させる
        /// </summary>
        /// <param name="degree">絶対的な角度</param>
        /// <param name="duration"></param>
        public Tween RollAbsolutely(IRoleOperation operation, float degree, float duration)
        {
            //print($"{role.Value}を{_roleDegreeTable.Value[role]}から{_roleDegreeTable.Value.InitTable[role] + degree}に回転します");
            return DOTween.To(() => _roleDegreeTable.Value[operation],
                                newDeg =>
                                {
                                    ApplyAngle(operation, newDeg);
                                },
                                _roleDegreeTable.Value.InitTable[operation] + degree, duration);
        }

        public Tween BringToFrontTw(IRoleOperation operation, RoleValue bringToRole, float duration)
        {
            return RollAbsolutely(operation,
                                  BringToFront0CorrectDegreeAngle - 360 / m_reelOperation.Roles.Count * GetRoleIndex(bringToRole),
                                  duration);
        }

        [Button("回転後に指定位置で停止")]
        private void RollAndStop(int times, RoleValue stopRole)
        {
            foreach (var operation in m_reelOperation.Roles)
            {
                DOTween.Sequence()
                       .Append(SimpleRotate(operation, 10, 3f))
                       .Append(BringToFrontTw(operation, stopRole, 1f))
                       .Play();
            }
        }

        private Tween SimpleRotate(IRoleOperation operation, int times, float duration)
        {
            return DOTween.To(() => _roleDegreeTable.Value[operation],
                                    newDeg => ApplyAngle(operation, newDeg),
                                    _roleDegreeTable.Value[operation] + 360 * times, duration);
        }

        private void OnDegreeChanged()
        {
            foreach (var role in m_reelOperation.Roles)
            {
                ApplyAngle(role,_roleDegreeTable.Value.InitTable[role] + m_degree);
            }
        }

        /// <summary>
        /// RoleValueからIRoleOperationを探して取得する
        /// </summary>
        /// <param name="roleValue"></param>
        /// <returns></returns>
        private IRoleOperation FindOperation(RoleValue roleValue)
        {
            return m_reelOperation.Roles.First(ope => ope.Value == roleValue);
        }
        private int GetRoleIndex(RoleValue roleValue)
        {
            return m_reelOperation.Roles
                                  .Select((ope, index) => new { ope, index })
                                  .Where(pair => pair.ope.Value == roleValue)
                                  .Select(pair => pair.index)
                                  .First();
        }

        /// <summary>
        /// 指定の角度にRoleを回転する
        /// </summary>
        /// <param name="targetDegree"></param>
        private void ApplyAngle(IRoleOperation role, float targetDegree)
        {
            //Roleのpositionを回転させる
            role.transform.position = new Vector3(
                0,
                m_radius * Mathf.Cos(targetDegree.ToRadian()),
                m_radius * Mathf.Sin(targetDegree.ToRadian())) + _roleInitialPositionTable.Value[role];
            //テーブルを更新
            _roleDegreeTable.Value[role] = targetDegree;
        }

        private class RoleInfoTable<T>
        {
            protected readonly IReadOnlyDictionary<IRoleOperation, T> m_initialTable;
            protected readonly Dictionary<IRoleOperation, T> m_operationTable;
            protected readonly Dictionary<RoleValue, T> m_roleValueTable;

            public RoleInfoTable(IReadOnlyDictionary<IRoleOperation, T> initialTable)
            {
                m_initialTable = initialTable;
                m_operationTable = initialTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                m_roleValueTable = initialTable.ToDictionary(kvp => kvp.Key.Value, kvp => kvp.Value);
            }

            public virtual T this[IRoleOperation index]
            {
                get => m_operationTable[index];
                set => m_operationTable[index] = value;
            }
            public virtual T this[RoleValue index]
            {
                get => m_roleValueTable[index];
                set => m_roleValueTable[index] = value;
            }

            public IReadOnlyDictionary<IRoleOperation, T> InitTable => m_initialTable;
        }

        private class RoleDegreeTable : RoleInfoTable<float>
        {
            public RoleDegreeTable(IReadOnlyDictionary<IRoleOperation, float> initialTable) : base(initialTable)
            {
            }

            public override float this[IRoleOperation index]
            {
                get => m_operationTable[index];
                set => m_operationTable[index] = CorrectDegreeRange(value);
            }
            public override float this[RoleValue index]
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