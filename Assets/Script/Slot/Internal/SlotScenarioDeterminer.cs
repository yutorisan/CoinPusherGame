using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using MedalPusher.Slot.Internal;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace MedalPusher.Slot.Internal
{
    /// <summary>
    /// スロットのシナリオを決定する
    /// </summary>
    internal class SlotScenarioDeterminer : MonoBehaviour, ISlotStarter
    {
        [Inject]
        private ISlotProductionDeterminer m_productionDeterminer;

        /// <summary>
        /// 直接当たる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("直接当たる確率"), BoxGroup("MECE")]
        private float directWinningProbability = 0.01f;
        /// <summary>
        /// リーチになる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("リーチになる確率"), BoxGroup("MECE")]
        private float reachProbability = 0.1f;
        /// <summary>
        /// リーチ状態から当たりになる確率
        /// </summary>
        [SerializeField, Range(0f, 1f), LabelText("リーチ状態から当たる確率"),]
        private float reachWinningProbability = 0.5f;

        private SlotScenarioGeneratorSelector generatorSelector;

        /// <summary>
        /// はずれる確率（インスペクタに表示用の読み取り専用プロパティ）
        /// </summary>
        [ShowInInspector, PropertyRange(0f, 1f), LabelText("はずれる確率"), BoxGroup("MECE")]
        private float loseProbability => 1f - directWinningProbability - reachProbability;

        // Start is called before the first frame update
        void Start()
        {
            generatorSelector = new SlotScenarioGeneratorSelector(this);
        }

        public UniTask<SlotResult> StartAsync()
        {
            //シナリオを決定
            var scenario = generatorSelector.Select().GenerateScenario();
            //シナリオに沿った演出の決定を依頼
            return m_productionDeterminer.DetermineProduction(scenario)
                                         .ContinueWith(() => scenario.Result);
        }

        /// <summary>
        /// スロットのシナリオを生成できる
        /// </summary>
        private interface ISlotScenarioGenerator
        {
            Scenario GenerateScenario();
        }
        /// <summary>
        /// 適切なISlotRoleSetGeneratorを選択する
        /// </summary>
        private class SlotScenarioGeneratorSelector
        {
            private readonly SlotScenarioDeterminer parent;
            public SlotScenarioGeneratorSelector(SlotScenarioDeterminer parent) => this.parent = parent;

            /// <summary>
            /// 設定された確率に応じて抽選されたSlotRoleSetGeneratorを選択する
            /// </summary>
            /// <returns></returns>
            public ISlotScenarioGenerator Select()
            {
                //設定確率によって決められたISlotScenarioGeneratorを返す
                switch (UnityEngine.Random.value)
                {
                    //ダイレクトあたり
                    case float p when p < parent.directWinningProbability:
                        return DirectWinScenarioGenerator.GetInstance();
                    //リーチ
                    case float p when p < parent.reachProbability:
                        return ReachScenarioGenerator.GetInstance(parent.reachWinningProbability);
                    //はずれ
                    default:
                        return LoseScenarioGenerator.GetInstance();
                }
            }

            /// <summary>
            /// 当たりとなるようなスロットの出目を生成する
            /// </summary>
            private class DirectWinScenarioGenerator : ISlotScenarioGenerator
            {
                private DirectWinScenarioGenerator() { }
                private static DirectWinScenarioGenerator _instance;
                public static ISlotScenarioGenerator GetInstance() => _instance ?? (_instance = new DirectWinScenarioGenerator());

                public Scenario GenerateScenario()
                {
                    return Scenario.DirectWin(RoleValue.FromRandom());
                }
            }
            /// <summary>
            /// リーチとなるようなスロットの出目を生成する
            /// </summary>
            private class ReachScenarioGenerator : ISlotScenarioGenerator
            {
                /// <summary>
                /// リーチから当たりになる確率
                /// </summary>
                private float reachToWinProbability;

                private ReachScenarioGenerator(float reachToWinPropability) => this.reachToWinProbability = reachToWinPropability;
                private static ReachScenarioGenerator _instance;
                public static ISlotScenarioGenerator GetInstance(float reachToWinProbability)
                {
                    if (_instance == null) return new ReachScenarioGenerator(reachToWinProbability);
                    else
                    {
                        _instance.reachToWinProbability = reachToWinProbability;
                        return _instance;
                    }
                }

                public Scenario GenerateScenario()
                {
                    //リーチする役を決定
                    RoleValue reachRole = RoleValue.FromRandom();
                    //外れる役を決定
                    RoleValue loseRole = RoleValue.FromRandom(reachRole);

                    //リーチ後に当たるかはずれるかを決定
                    bool isWin = UnityEngine.Random.value < reachToWinProbability;

                    //リーチ後の確定役を決定
                    RoleValue afterReachRole;
                    //あたりなら揃える
                    if (isWin) afterReachRole = reachRole;
                    //ハズレならその前の役にする
                    else afterReachRole = reachRole.Previous;

                    //はずれる位置を決定
                    switch (UnityEngine.Random.Range(0,3))
                    {
                        case 0:
                            return Scenario.Reach(new RoleSet(loseRole, reachRole, reachRole), afterReachRole);
                        case 1:
                            return Scenario.Reach(new RoleSet(reachRole, loseRole, reachRole), afterReachRole);
                        case 2:
                            return Scenario.Reach(new RoleSet(reachRole, reachRole, loseRole), afterReachRole);
                        default:
                            throw new Exception("リーチの位置を決定できませんでした");
                    }
                }
            }
            /// <summary>
            /// はずれとなるようなスロットの出目を生成する
            /// </summary>
            private class LoseScenarioGenerator : ISlotScenarioGenerator
            {
                private LoseScenarioGenerator() { }
                private static LoseScenarioGenerator _instance;
                public static ISlotScenarioGenerator GetInstance() => _instance ?? (_instance = new LoseScenarioGenerator());

                public Scenario GenerateScenario()
                {
                    var role1 = RoleValue.FromRandom();
                    var role2 = RoleValue.FromRandom(role1);
                    var role3 = RoleValue.FromRandom(role1, role2);
                    return Scenario.Lose(new RoleSet(role1, role2, role3));
                }
            }
        }
    }

    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    internal interface ISlotProductionDeterminer
    {
        /// <summary>
        /// スロットの演出の決定を依頼する
        /// </summary>
        /// <param name="scenario">スロットのシナリオ</param>
        /// <returns>スロットの演出を決定して回したスロットの回転が完了するまでのタスク</returns>
        UniTask DetermineProduction(Scenario scenario);
    }
}