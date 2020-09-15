using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;
using UniRx;

namespace MedalPusher.Item.Payout
{
    public class PayoutMedalView : MonoBehaviour
    {
        [Inject]
        private IObservableMedalPayouter _observablePayouter;
        [SerializeField]
        private TextMeshProUGUI m_text;

        private void Start()
        {
            _observablePayouter.PayoutStockMedals
                               .Subscribe(medals => m_text.text = $"WIN:{medals}");
        }


    }
}