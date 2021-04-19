using Zenject;
using MedalPusher.Slot.Internal.Stock;
using MedalPusher.Slot.Internal;

namespace MedalPusher.Slot
{
    public class SlotInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            StockCounter stockCounter = new StockCounter();
            //StockCounter
            Container.Bind<IStockCounter>()
                     .To<StockCounter>()
                     .FromInstance(stockCounter)
                     .AsCached();
            Container.Bind<IReadOnlyObservableStockCount>()
                     .To<StockCounter>()
                     .FromInstance(stockCounter)
                     .AsCached();
            Container.Bind<IStockAdder>()
                     .To<StockCounter>()
                     .FromInstance(stockCounter)
                     .AsCached();

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