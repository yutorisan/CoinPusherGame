using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロット
    /// </summary>
    public class Slot : SerializedMonoBehaviour
    {
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_reftReel;
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_centerReel;
        [SerializeField, BoxGroup("Reels")]
        private readonly IObservableReelDecided m_rightReel;


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