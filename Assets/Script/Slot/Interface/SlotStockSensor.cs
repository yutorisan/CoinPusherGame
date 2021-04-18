using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using Sirenix.OdinInspector;
using MedalPusher.Item.Checker;
using Zenject;
using MedalPusher.Slot.Internal.Stock;

namespace MedalPusher.Slot.Interface
{
    /// <summary>
    /// スロットのストック追加チェッカーを監視して、ストックを追加する
    /// </summary>
    public class SlotStockSensor : SerializedMonoBehaviour
    {
        /// <summary>
        /// スロットのストックを追加するメダルチェッカー
        /// </summary>
        [SerializeField, Required]
        private IObservableMedalChecker stockMedalChecker;
        /// <summary>
        /// ストックを追加する窓口
        /// </summary>
        [Inject]
        private IStockAdder stockAdder;

        private void Start()
        {
            //チェッカーが反応したらストックを追加する
            stockMedalChecker.Checked.Subscribe(_ => stockAdder.Add());
        }
    }
}