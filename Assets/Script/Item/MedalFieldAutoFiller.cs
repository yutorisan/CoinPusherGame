using System.Collections;
using System.Collections.Generic;
using MedalPusher.Debug;
using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

namespace MedalPusher.Item
{
    /// <summary>
    /// 自動的にフィールド上にメダルを充填する
    /// </summary>
    public class MedalFieldAutoFiller : DebugSettingFacadeTargetMonoBehaviour, IInitialFillMedalCountSetterFacade
    {
        /// <summary>
        /// メダルのPayouter
        /// </summary>
        [Inject]
        private IMedalPayoutOperation medalPayouter;
        /// <summary>
        /// 充填するメダル数
        /// </summary>
        [SerializeField]
        private int initialFillMedals;

        public void SetInitialFillMedalCount(int count)
        {
            initialFillMedals = count;
        }

        async void Start()
        {
            //DebugFacadeによる設定を待機する
            await this.FacadeAlreadyOKAsync;
            //充填を実行する
            medalPayouter.PayoutRequest(initialFillMedals, MedalPayoutMethod.Shower);
        }
    }
}