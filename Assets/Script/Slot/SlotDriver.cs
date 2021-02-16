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
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_leftReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_middleReel;
        [SerializeField, TitleGroup("ReelObjects")]
        private GameObject m_rightReel;

        [SerializeField, TitleGroup("NormalRollProperty"), LabelText("周回数")]
        private int m_normalRollLaps;
        [SerializeField, TitleGroup("NormalRollProperty"), LabelText("最大回転速度(rps)")]
        private float m_normalRollMaxRps;
        [SerializeField, TitleGroup("NormalRollProperty"), LabelText("加速時間")]
        private float m_normalRollAccelDuration;
        [SerializeField, TitleGroup("NormalRollProperty"), LabelText("減速時間")]
        private float m_normalRollDeceleDuration;

        private IReadOnlyDictionary<ReelPos, IReelSequenceProvider> m_sequenceProviderTable;

        private void Start()
        {
            //各子オブジェクトのRoleコンポーネントを取得
            var leftRoles = m_leftReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var middleRoles = m_middleReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();
            var rightRoles = m_rightReel.transform.Cast<Transform>().Select(t => t.GetComponent<IRoleOperation>()).ToArray();

            m_sequenceProviderTable = new Dictionary<ReelPos, IReelSequenceProvider>()
            {
                {ReelPos.Left, new ReelSequenceProvider(leftRoles, 0.2f) },
                {ReelPos.Middle, new ReelSequenceProvider(middleRoles, 0.2f) },
                {ReelPos.Right, new ReelSequenceProvider(rightRoles, 0.2f) }
            };
        }

        public UniTask ControlBy(Production production)
        {
            List<UniTask> rollAndReachSqTasks = new List<UniTask>();
            foreach (var item in m_sequenceProviderTable)
            {
                var reelPos = item.Key;
                var provider = item.Value;
                //通常の回転シーケンスを取得
                var sq = provider.GetNormalRollSequence(production.Scenario.GetReelScenario(reelPos).FirstRoleValue,
                                                             production.NormalProperty);
                //リーチの場合はリーチシーケンスを追加
                if (production.GetReelProduction(reelPos).ReelScenario.IsReachReelScenario)
                {
                    var reelProduction = production.GetReelProduction(reelPos);
                    sq.Append(provider.GetReachReRollSequence(reelProduction.ReelScenario.FirstRoleValue,
                                                                   reelProduction.ReelScenario.AfterReachRole.Value,
                                                                   production.ReachProperty));
                }
                rollAndReachSqTasks.Add(sq.PlayAsync());
            }

            return UniTask.WhenAll(rollAndReachSqTasks).ContinueWith(() =>
            {
                //当たりじゃなければここで終了
                if (!production.Scenario.IsWinScenario) return UniTask.CompletedTask;
                //当たりなら当たり演出を追加
                List<UniTask> winTasks = new List<UniTask>();
                foreach (var item in m_sequenceProviderTable)
                {
                    var reelPos = item.Key;
                    var provider = item.Value;
                    winTasks.Add(provider.GetWinningProductionSequence(production.Scenario.FinalRoleset[reelPos]).PlayAsync());
                }
                return UniTask.WhenAll(winTasks);
            });
        }
    }


}