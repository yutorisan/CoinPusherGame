using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot.Stock;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot
{
    /// <summary>
    /// スロットの抽選を開始させる
    /// </summary>
    public class SlotStarter : MonoBehaviour
    {
        [Inject]
        private ISlotRoleDeterminer _roleDeterminer;
        [Inject]
        private IObservableStockCount _observableStock;
        [Inject]
        private IObservableSlotStatus _slotStatus;

        // Start is called before the first frame update
        void Start()
        {
            //ストックが追加された場合と、
            _observableStock.Stock
                            .Pairwise()
                            //スロットが稼働中でない場合のみ
                            .Where(_ => _slotStatus.Status.Value == SlotStatus.Idol)
                            //前よりストックが増えている
                            .Where(pair => pair.Current > pair.Previous)
                            .AsUnitObservable()
                            .Debug()
                            .Merge(
            //ストックが溜まっている場合にIdol状態になった場合に、
                _slotStatus.Status
                           .Where(status => status == SlotStatus.Idol)
                           .Where(_ => _observableStock.Stock.Value > 0)
                           .AsUnitObservable())
            //スロットを回転させる（役の決定を依頼する）
                           .Subscribe(_ => _roleDeterminer.DetermineRole());
                                  
        }

        [Button]
        private void ForceStart()
        {
            _roleDeterminer.DetermineRole();
        }
    }
}