using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの演出を決定できる
    /// </summary>
    public interface ISlotProductionDeterminer
    {
        void DetermineProduction(RoleSet roleSet);
    }
    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    public class SlotProductionDeterminer : SerializedMonoBehaviour, ISlotProductionDeterminer
    {
        [SerializeField]
        private readonly IReadOnlyDictionary<ReelPos, IReelDriver> m_reelControllers = new Dictionary<ReelPos, IReelDriver>();

        public void DetermineProduction(RoleSet roleSet)
        {
            throw new System.NotImplementedException();
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}