using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot.Internal.Stock
{
    public class StockSettingProvider
    {
        [Inject]
        private IReadOnlyStockLevelSetting stockLevelSetting;

        private static StockSettingProvider instance;
        public static StockSettingProvider Instance => instance ??= new StockSettingProvider();

        public IReadOnlyList<StockLevelInfo> Setting => stockLevelSetting.StockLevelSettingList;
        public int MaxLevel => Setting.Count;
    }
}