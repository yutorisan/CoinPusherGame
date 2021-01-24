using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Prize;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの出目を決定できる
    /// </summary>
    public interface ISlotRoleDeterminer
    {
        /// <summary>
        /// 出目の決定を依頼する
        /// </summary>
        /// <returns>依頼した出目決定によってスロットが回転し、その出目がでたことの通知</returns>
        IObservable<Unit> DetermineRole();
    }
    /// <summary>
    /// スロットの出目を決定する
    /// </summary>
    public class SlotRoleDeterminer : MonoBehaviour, ISlotRoleDeterminer
    {
        [Inject]
        private ISlotProductionDeterminer m_productionDeterminer;
        [Inject]
        private ISlotPrizeOrderer m_prizeOrderer;

        /// <summary>
        /// 直接当たる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("直接当たる確率"), BoxGroup("MECE")]
        private float m_directWinningProbability = 0.01f;
        /// <summary>
        /// リーチになる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("リーチになる確率"), BoxGroup("MECE")]
        private float m_reachProbability = 0.1f;
        /// <summary>
        /// リーチ状態から当たりになる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("リーチ状態から当たる確率"),]
        private float m_reachWinningProbability = 0.5f;

        private SlotRoleSetGeneratorSelector m_generatorSelector;

        /// <summary>
        /// はずれる確率
        /// </summary>
        [ShowInInspector, PropertyRange(0f, 1f), LabelText("はずれる確率"), BoxGroup("MECE")]
        private float loseProbability => 1f - m_directWinningProbability - m_reachProbability;

        // Start is called before the first frame update
        void Start()
        {
            m_generatorSelector = new SlotRoleSetGeneratorSelector(this);
        }

        public IObservable<Unit> DetermineRole()
        {
            //出目を決定
            var roleset = m_generatorSelector.Select().GenerateRoleSet();
            //出目に応じて演出の決定を依頼
            var done = m_productionDeterminer.DetermineProduction(roleset);
            //依頼した演出が終了したら、景品の払い出しを依頼
            done.Subscribe(_ => m_prizeOrderer.Order(roleset));

            return done;
        }

        /// <summary>
        /// スロットの出目を生成できる
        /// </summary>
        private interface ISlotRoleSetGenerator
        {
            RoleSet GenerateRoleSet();
        }
        /// <summary>
        /// 適切なISlotRoleSetGeneratorを選択する
        /// </summary>
        private class SlotRoleSetGeneratorSelector
        {
            private SlotRoleDeterminer _parent;
            public SlotRoleSetGeneratorSelector(SlotRoleDeterminer parent) => _parent = parent;

            /// <summary>
            /// 設定された確率に応じて抽選されたSlotRoleSetGeneratorを選択する
            /// </summary>
            /// <returns></returns>
            public ISlotRoleSetGenerator Select()
            {
                switch (UnityEngine.Random.value)
                {
                    //ダイレクトあたり
                    case float p when p < _parent.m_directWinningProbability:
                        return DirectWinRoleSetGenerator.Instance;
                    //リーチ
                    case float p when p < _parent.m_reachProbability:
                        return ReachRoleSetGenerator.Instance;
                    //はずれ
                    default:
                        return LoseRoleSetGenerator.Instance;
                }
            }

            /// <summary>
            /// 当たりとなるようなスロットの出目を生成する
            /// </summary>
            private class DirectWinRoleSetGenerator : ISlotRoleSetGenerator
            {
                private DirectWinRoleSetGenerator() { }
                private static DirectWinRoleSetGenerator _instance;
                public static ISlotRoleSetGenerator Instance => _instance ?? (_instance = new DirectWinRoleSetGenerator());
                public RoleSet GenerateRoleSet()
                {
                    RoleValue role = RoleValue.FromRandom();
                    return new RoleSet(role, role, role);
                }
            }
            /// <summary>
            /// リーチとなるようなスロットの出目を生成する
            /// </summary>
            private class ReachRoleSetGenerator : ISlotRoleSetGenerator
            {
                private ReachRoleSetGenerator() { }
                private static ReachRoleSetGenerator _instance;
                public static ISlotRoleSetGenerator Instance => _instance ?? (_instance = new ReachRoleSetGenerator());
                public RoleSet GenerateRoleSet()
                {
                    //リーチする役を決定
                    RoleValue reachRole = RoleValue.FromRandom();
                    //外れる役を決定（偶然当たることもあるが織り込み済みとする）
                    RoleValue loseRole = RoleValue.FromRandom();
                    //はずれる位置を決定
                    switch (UnityEngine.Random.Range(0,3))
                    {
                        case 0:
                            return new RoleSet(loseRole, reachRole, reachRole);
                        case 1:
                            return new RoleSet(reachRole, loseRole, reachRole);
                        case 2:
                            return new RoleSet(reachRole, reachRole, loseRole);
                        default:
                            throw new System.Exception("リーチの位置を決定できませんでした");
                    }
                }
            }
            /// <summary>
            /// はずれとなるようなスロットの出目を生成する
            /// </summary>
            private class LoseRoleSetGenerator : ISlotRoleSetGenerator
            {
                private LoseRoleSetGenerator() { }
                private static LoseRoleSetGenerator _instance;
                public static ISlotRoleSetGenerator Instance => _instance ?? (_instance = new LoseRoleSetGenerator());
                public RoleSet GenerateRoleSet()
                {
                    return new RoleSet(RoleValue.FromRandom(), RoleValue.FromRandom(), RoleValue.FromRandom());
                }
            }
        }
    }
}