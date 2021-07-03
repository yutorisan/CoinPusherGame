using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Slot.Internal.Stock
{
    /// <summary>
    /// Stockの設定値を読み取ることができる
    /// </summary>
    internal interface IReadOnlyStockLevelSetting
    {
        IReadOnlyList<StockLevelInfo> StockLevelSettingList { get; }
    }
    /// <summary>
    /// インスペクタで<see cref="StockLevelInfo"/>の設定を行うためのクラス
    /// </summary>
    public class StockLevelSetting : MonoBehaviour, IReadOnlyStockLevelSetting
    {
        [SerializeField]
        private List<StockLevelInfo> stockLevels;

        public IReadOnlyList<StockLevelInfo> StockLevelSettingList => stockLevels;
    }
}