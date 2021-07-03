using Zenject;
using MedalPusher.Slot.Internal.Stock;
using MedalPusher.Slot.Internal;
using UnityEngine;

namespace MedalPusher.Slot
{
    public class SlotInstaller : MonoInstaller
    {
        [SerializeField]
        private StockLevelSetting stockLevelSetting;

        public override void InstallBindings()
        {
            StockList stockList = new StockList(stockLevelSetting);
            //StockList
            Container.Bind<IStockList>()
                     .To<StockList>()
                     .FromInstance(stockList)
                     .AsCached();
            Container.Bind<IObservableStockCount>()
                     .To<StockList>()
                     .FromInstance(stockList)
                     .AsCached();
            Container.Bind<IObservableStockList>()
                     .To<StockList>()
                     .FromInstance(stockList)
                     .AsCached();
            Container.Bind<IStockAdder>()
                     .To<StockList>()
                     .FromInstance(stockList)
                     .AsCached();
            //other
            Container.Bind<ISlotResultSubmitter>()
                     .To<SlotStartScheduler>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<ISlotStarter>()
                     .To<SlotScenarioDeterminer>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<ISlotProductionDeterminer>()
                     .To<SlotProductionDeterminer>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<ISlotDriver>()
                     .To<SlotDriver>()
                     .FromComponentInHierarchy()
                     .AsCached();


        }
    }
}