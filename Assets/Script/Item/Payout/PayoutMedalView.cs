using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;
using UniRx;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// 払い出し中のメダル数を表示する
    /// </summary>
    public class PayoutMedalView : MonoBehaviour
    {
        /// <summary>
        /// 払い出し中のメダル数の提供元
        /// </summary>
        [Inject]
        private IObservableMedalPayoutStock observablePayoutStock;
        /// <summary>
        /// 表示するText
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI text;

        private void Start()
        {
            observablePayoutStock.PayoutStock.Subscribe(medals => text.text = $"WIN:{medals}");
        }


    }
}