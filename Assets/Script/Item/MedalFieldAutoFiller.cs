using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

namespace MedalPusher.Item
{
    public class MedalFieldAutoFiller : MonoBehaviour
    {
        [Inject]
        private IMedalPayoutOperation m_medalPayouter;

        [SerializeField]
        private int m_initialMedals;
        // Start is called before the first frame update
        void Start()
        {
            m_medalPayouter.PayoutRequest(m_initialMedals, MedalPayoutMethod.Shower);
        }
    }
}