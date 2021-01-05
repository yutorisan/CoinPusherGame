using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot.Stock
{
    public class StockCounter : MonoBehaviour
    {
        [Inject]
        private ISlotStarter _slotStarter;

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