using MedalPusher.Slot.Prize;
using MedalPusher.Slot.Stock;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    public class SlotInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IStockCounter>()
                     .To<StockCounter>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IReadOnlyObservableStockCount>()
                     .To<StockCounter>()
                     .FromComponentsInHierarchy()
                     .AsCached();
            Container.Bind<ISlotScenarioDeterminer>()
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
            Container.Bind<IReadOnlyObservableSlotProdctionStatus>()
                     .To<SlotDriver>()
                     .FromComponentInHierarchy()
                     .AsCached();

            Container.Bind<ISlotPrizeOrderer>()
                     .To<SlotPrizeOrderer>()
                     .AsTransient();
        }
    }
}