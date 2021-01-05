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

    }
    /// <summary>
    /// スロットの演出を決定する
    /// </summary>
    public class SlotProductionDeterminer : SerializedMonoBehaviour
    {
        [SerializeField, BoxGroup("Reel")]
        private readonly IReelController m_leftReelController;
        [SerializeField, BoxGroup("Reel")]
        private readonly IReelController m_centerReelController;
        [SerializeField, BoxGroup("Reel")]
        private readonly IReelController m_rightReelController;

        

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