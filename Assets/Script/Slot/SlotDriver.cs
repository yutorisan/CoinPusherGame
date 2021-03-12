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
    public class SlotDriver : MonoBehaviour, ISlotDriver
    {
        public static readonly float SlotRadius = .2f;

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

        public UniTask ControlBy(Production production)
        {
            print(production.FinallyRoleset);
            List<UniTask> rollAndReachSqTasks = new List<UniTask>();
            foreach (var item in m_sequenceProviderTable)
            {
                var reelPos = item.Key;
                var provider = item.Value;
                //通常の回転シーケンスを取得
                var sq = ReelSequence.Empty().Append( provider.CreateFirstRollSequence(production[reelPos].FirstRole,
                                                           production.NormalProperty));
                //リーチの場合はリーチシーケンスを追加
                if (production[reelPos].IsReachProduction)
                {
                    sq.OnComplete(() => print("リーチに入りました!"));

                    ReelProduction reelProduction = production[reelPos];
                    RoleValue defenser, offenser;

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
                if (!production.IsWinProduction) return UniTask.CompletedTask;
                //当たりなら当たり演出を追加
                List<UniTask> winTasks = new List<UniTask>();
                foreach (var item in m_sequenceProviderTable)
                {
                    var reelPos = item.Key;
                    var provider = item.Value;
                    winTasks.Add(provider.CreateWinningProductionSequence(production.FinallyRoleset[reelPos]).PlayAsync());
                }
                return UniTask.WhenAll(winTasks);
            });
        }
    }


}