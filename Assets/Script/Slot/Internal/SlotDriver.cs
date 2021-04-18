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
using MedalPusher.Slot.Internal.Core;
using UnityUtility;
using MedalPusher.Utils;

namespace MedalPusher.Slot.Internal
{
    /// <summary>
    /// スロットの演出状況を取得または購読できる
    /// </summary>
    internal interface IReadOnlyObservableSlotProdctionStatus
    {
        /// <summary>
        /// 現在のスロットの演出状況を取得または購読する
        /// </summary>
        IReadOnlyReactiveProperty<SlotProductionStatus> ProductionStatus { get; }
    }
    /// <summary>
    /// スロットの動きを制御する
    /// </summary>
    internal class SlotDriver : MonoBehaviour, ISlotDriver, IReadOnlyObservableSlotProdctionStatus
    {
        /// <summary>
        /// スロットリールの半径
        /// </summary>
        public static readonly float SlotRadius = .2f;
        /// <summary>
        /// 現在のスロットの演出状況
        /// </summary>
        private readonly IReactiveProperty<SlotProductionStatus> productionStatus = new ReactiveProperty<SlotProductionStatus>();
        /// <summary>
        /// 左右中央の各リールに対する制御シーケンスの提供者
        /// </summary>
        private IReadOnlyDictionary<ReelPos, IReelSequenceProvider> sequenceProviderTable;

        //各リールのオブジェクト
        [SerializeField, TitleGroup("ReelObjects"), Required]
        private GameObject m_leftReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_middleReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_rightReel;


        private void Start()
        {
            //各子オブジェクトのRoleコンポーネントを取得
            var leftRoles = m_leftReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var middleRoles = m_middleReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var rightRoles = m_rightReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();

            //リールに対する制御シーケンスを提供するReelSequenceProviderを、左右中央の3つのリールに対して生成
            sequenceProviderTable = new Dictionary<ReelPos, IReelSequenceProvider>()
            {
                {ReelPos.Left, new ReelSequenceProvider(leftRoles, SlotRadius) },
                {ReelPos.Middle, new ReelSequenceProvider(middleRoles, SlotRadius) },
                {ReelPos.Right, new ReelSequenceProvider(rightRoles, SlotRadius) }
            };
        }

        public IReadOnlyReactiveProperty<SlotProductionStatus> ProductionStatus => productionStatus;

        public UniTask DriveBy(Production production)
        {
            //各リールの制御タスクを格納するListを用意
            List<UniTask> reelDriveTasks = new List<UniTask>();

            //左右真ん中3つのリールに対して、同様の処理を行う
            foreach (var (reelPos, provider) in sequenceProviderTable)
            {
                //通常の回転シーケンスを取得
                var sq = provider.CreateFirstRollSequence(production[reelPos].FirstRole,
                                                          production.NormalProperty)
                                 //Play時にステータスを更新
                                 .OnPlay(() => productionStatus.Value = SlotProductionStatus.Rolling);

                //リールの演出がリーチの場合はリーチシーケンスを追加
                if (production[reelPos].IsReachProduction)
                {
                    //リーチ開始時（通常の回転シーケンス完了時）にステータスを更新
                    sq.OnComplete(() => productionStatus.Value = SlotProductionStatus.Reaching);

                    //リールの演出を取得
                    ReelProduction reelProduction = production[reelPos];
                    //拮抗演出の守備側と攻撃側を取得
                    //defenser:現在の位置を死守しようとする役, offenser:位置を獲得しようとする役
                    RoleValue defenser = production.IsWinProduction ? reelProduction.FinallyRole.Previous :
                                                                      reelProduction.FinallyRole;
                    RoleValue offenser = production.IsWinProduction ? reelProduction.FinallyRole :
                                                                      reelProduction.FinallyRole.Next; 
                    //拮抗直前まで高速回転させるシーケンスを追加
                    sq.Append(provider.CreateReachReRollSequence(startRole: reelProduction.FirstRole,
                                                                 endValue: defenser,
                                                                 prop: production.ReachProperty));
                    //ランダムに拮抗演出を取得する
                    AntagonistType antagonistType = UnityUtility.Enum.Random<AntagonistType>();

                    //拮抗演出シーケンスを追加
                    sq.Append(provider.CreateAntagonistSequence(antagonistType, new AntagonistSequenceProperty()
                    {
                        Defenser = defenser,
                        Offenser = offenser,
                        IsOffenserWin = production.IsWinProduction
                    }));
                }

                //リールの制御を開始して、そのタスクを保持する
                reelDriveTasks.Add(sq.PlayAsync());
            }

            //各リールの制御がすべて完了したら、当たり演出を追加する
            //当たり演出が完了するまでのタスクを返す
            return UniTask.WhenAll(reelDriveTasks).ContinueWith(() =>
            {
                //当たりじゃなければここで終了
                if (!production.IsWinProduction)
                {
                    productionStatus.Value = SlotProductionStatus.Idol;
                    return UniTask.CompletedTask;
                }
                //当たりなら当たり演出を追加
                else
                {
                    //各リールに対して当たり演出シーケンスを追加
                    return sequenceProviderTable
                        .Select(kvp => (reelpos: kvp.Key, provider: kvp.Value))
                        .Select(pair => pair.provider
                                            //当たり演出シーケンスを取得
                                            .CreateWinningProductionSequence(production.FinallyRoleset[pair.reelpos])
                                            //再生開始したらステータスを「当たり演出中」に変更
                                            .OnPlay(() => productionStatus.Value = SlotProductionStatus.Winning)
                                            //シーケンスが完了したらステータスを「アイドル」に変更
                                            .OnComplete(() => productionStatus.Value = SlotProductionStatus.Idol)
                                            //再生してタスクに変換
                                            .PlayAsync())
                        //各リールのタスクをまとめて返す
                        .WhenAll();
                }
            });
        }
    }

}