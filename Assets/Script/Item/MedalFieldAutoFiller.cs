using System.Collections;
using System.Collections.Generic;
using MedalPusher.Debug;
using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

namespace MedalPusher.Item
{
    public class MedalFieldAutoFiller : DebugSettingFacadeTargetMonoBehaviour, IInitialFillMedalCountSetterFacade
    {
        [Inject]
        private IMedalPayoutOperation m_medalPayouter;

        [SerializeField]
        private int m_initialMedals;

        public void SetInitialFillMedalCount(int count)
        {
            m_initialMedals = count;
        }

        // Start is called before the first frame update
        async void Start()
        {
            await this.FacadeAlreadyOKAsync;

            m_medalPayouter.PayoutRequest(m_initialMedals, MedalPayoutMethod.Shower);
        }
    }
}