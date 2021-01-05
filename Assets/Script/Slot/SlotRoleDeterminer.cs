using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの役を決定できる
    /// </summary>
    public interface ISlotRoleDeterminer
    {

    }
    /// <summary>
    /// スロットの役を決定する
    /// </summary>
    public class SlotRoleDeterminer : MonoBehaviour, ISlotRoleDeterminer
    {
        [Inject]
        private ISlotProductionDeterminer _productionDeterminer;

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