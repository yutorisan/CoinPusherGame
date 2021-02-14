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
            List<UniTask> tasks = new List<UniTask>();
            foreach (var item in m_sequenceProviderTable.Select(kvp => new { pos = kvp.Key, provider = kvp.Value }))
            {
                //通常の回転シーケンスを取得
                var sq = item.provider.GetNormalRollSequence(production.Scenario.GetReelScenario(item.pos).FirstRoleValue,
                                                             production.NormalProperty);
                //リーチの場合はリーチシーケンスを追加
                if (production.GetReelProduction(item.pos).ReelScenario.IsReachReelScenario)
                {
                    var reelProduction = production.GetReelProduction(item.pos);
                    sq.Append(item.provider.GetReachReRollSequence(reelProduction.ReelScenario.FirstRoleValue,
                                                                   reelProduction.ReelScenario.AfterReachRole.Value,
                                                                   production.ReachProperty));
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