using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using Zenject;
using Sirenix.OdinInspector;

namespace MedalPusher.Slot.Internal.Stock
{
    /// <summary>
    /// デバッグ用
    /// スロットのストックを強制的に追加する
    /// </summary>
    public class DebugStockAdder : MonoBehaviour
    {
        [Inject]
        private IStockAdder adder;

        [Button("Add")]
        private void Add() => adder.Add();
    }
}