using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using MedalPusher.Slot.Sequences;
using UnityUtility;

namespace MedalPusher.Slot
{
    public interface ISlotDriver
    {
        /// <summary>
        /// 指定した演出に従ってスロットを制御します。
        /// </summary>
        /// <param name="production">演出</param>
        /// <returns>スロットの制御完了通知</returns>
        UniTask ControlBy(Production production);
    }
    public interface IObservableSlotProdctionStatus
    {
        /// <summary>
        /// スロット演出の状況とその変更通知を提供します
        /// </summary>
        IReadOnlyReactiveProperty<SlotProductionStatus> ProductionStatus { get; }
    }
    public class SlotDriver : MonoBehaviour, ISlotDriver, IObservableSlotProdctionStatus
    {
        /// <summary>
        /// スロットリールの半径
        /// </summary>
        public static readonly float SlotRadius = .2f;
        /// <summary>
        /// 現在のスロットの演出
        /// </summary>
        private readonly IReactiveProperty<SlotProductionStatus> m_productionStatus = new ReactiveProperty<SlotProductionStatus>();

        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_leftReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_middleReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_rightReel;

        private IReadOnlyDictionary<ReelPos, IReelSequenceProvider> m_sequenceProviderTable;

        private void Start()
        {
            //各子オブジェクトのRoleコンポーネントを取得
            var leftRoles = m_leftReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var middleRoles = m_middleReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var rightRoles = m_rightReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();

            m_sequenceProviderTable = new Dictionary<ReelPos, IReelSequenceProvider>()
            {
                {ReelPos.Left, new ReelSequenceProvider(leftRoles, SlotRadius) },
                {ReelPos.Middle, new ReelSequenceProvider(middleRoles, SlotRadius) },
                {ReelPos.Right, new ReelSequenceProvider(rightRoles, SlotRadius) }
            };
        }

        public IReadOnlyReactiveProperty<SlotProductionStatus> ProductionStatus => m_productionStatus;

        public UniTask ControlBy(Production production)
        {
            print(production.FinallyRoleset);

            //各Sequneceの再生Taskを格納するListを用意
            List<UniTask> rollAndReachSqTasks = new List<UniTask>();

            //左右真ん中3つのリールに対して、同様の処理を行う
            foreach (var (reelPos, provider) in m_sequenceProviderTable)
            {
                //通常の回転シーケンスを取得
                var sq = provider.CreateFirstRollSequence(production[reelPos].FirstRole,
                                                          production.NormalProperty)
                                 //Play時にステータスを更新
                                 .OnPlay(() => m_productionStatus.Value = SlotProductionStatus.Rolling);
                //リーチの場合はリーチシーケンスを追加
                if (production[reelPos].IsReachProduction)
                {
                    //リーチ開始時（通常の回転シーケンス完了時）にステータスを更新
                    sq.OnComplete(() => m_productionStatus.Value = SlotProductionStatus.Reaching);

                    ReelProduction reelProduction = production[reelPos];
                    RoleValue defenser, offenser; //defenser:現在の位置を死守しようとする役, offenser:位置を獲得しようとする役

                    if (production.IsWinProduction)
                    {
                        defenser = reelProduction.FinallyRole.Previous; 
                        offenser = reelProduction.FinallyRole;
                    }
                    else
                    {
                        defenser = reelProduction.FinallyRole;
                        offenser = reelProduction.FinallyRole.Next;
                    }

                    sq.Append(provider.CreateReachReRollSequence(startRole: reelProduction.FirstRole,
                                                                 endValue: defenser,
                                                                 prop: production.ReachProperty));
                    //ランダムに拮抗演出を取得する
                    AntagonistType antagonistType = UnityUtility.Enum.Random<AntagonistType>();

                    sq.Append(provider.CreateAntagonistSequence(antagonistType, new AntagonistSequenceProperty()
                    {
                        Defenser = defenser,
                        Offenser = offenser,
                        IsOffenserWin = production.IsWinProduction
                    }));

                    sq.OnComplete(() => print("リーチが終わりました！"));
                }
                rollAndReachSqTasks.Add(sq.PlayAsync());
            }

            return UniTask.WhenAll(rollAndReachSqTasks).ContinueWith(() =>
            {
                //当たりじゃなければここで終了
                if (!production.IsWinProduction)
                {
                    m_productionStatus.Value = SlotProductionStatus.Idol;
                    return UniTask.CompletedTask;
                }
                //当たりなら当たり演出を追加
                else
                {
                    return m_sequenceProviderTable
                        .Select(kvp => (reelpos: kvp.Key, provider: kvp.Value))
                        .Select(pair => pair.provider
                                            .CreateWinningProductionSequence(production.FinallyRoleset[pair.reelpos])
                                            .OnPlay(() => m_productionStatus.Value = SlotProductionStatus.Winning)
                                            .OnComplete(() => m_productionStatus.Value = SlotProductionStatus.Idol)
                                            .PlayAsync())
                        .WhenAll();
                }
            });
        }
    }

}