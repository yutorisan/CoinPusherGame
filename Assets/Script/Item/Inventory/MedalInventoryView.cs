using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;
using UniRx;
using Sirenix.OdinInspector;

namespace MedalPusher.Item.Inventory
{
    /// <summary>
    /// メダルインベントリ内にメダル数を表示する
    /// </summary>
    public class MedalInventoryView : MonoBehaviour
    {
        [Inject]
        private IObservableMedalInventory medalInventory;
        [SerializeField, Required]
        private TextMeshProUGUI target;

        // Use this for initialization
        void Start()
        {
            medalInventory.ObservableMedalInventoryCount
                          .Subscribe(medals => target.text = medals.ToString());
        }
    }
}