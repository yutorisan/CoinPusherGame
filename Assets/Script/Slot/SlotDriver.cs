using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;

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
        [SerializeField]
        private GameObject m_leftReel;
        [SerializeField]
        private GameObject m_middleReel;
        [SerializeField]
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
                {ReelPos.Left, new ReelSequenceProvider(leftRoles, 0.2f) },
                {ReelPos.Middle, new ReelSequenceProvider(middleRoles, 0.2f) },
                {ReelPos.Right, new ReelSequenceProvider(rightRoles, 0.2f) }
            };
        }

        public UniTask ControlBy(Production production)
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach (var item in m_sequenceProviderTable.Select(kvp => new { pos = kvp.Key, provider = kvp.Value }))
            {
                var sq = item.provider.GetNormalRollSequence(production.Scenario.GetReelScenario(item.pos).FirstRoleValue, 5, 3, 2, 2);
                if (production.GetReelProduction(item.pos).ReelScenario.IsReachReelScenario)
                {
                    sq.Append(item.provider.GetReachReRollSequence(production.GetReelProduction(item.pos), 0.5f, 0.5f));
                }

                var task = sq.Play();

                tasks.Add(task);
            }



            return UniTask.WhenAll(tasks).ContinueWith(() =>
            {
                List<UniTask> wintasks = new List<UniTask>();
                foreach (var item in m_sequenceProviderTable.Select(kvp => new { pos = kvp.Key, provider = kvp.Value }))
                {
                    if (production.Scenario.IsWinScenario)
                    {
                        wintasks.Add(item.provider.GetWinningProductionSequence(production.Scenario.FinalRoleset[item.pos]).Play().AsyncWaitForCompletion().AsUniTask());
                    }
                }
                return UniTask.WhenAll(wintasks);
            });
        }
    }


}