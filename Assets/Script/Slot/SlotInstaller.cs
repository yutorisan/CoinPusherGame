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
            Container.Bind<IObservableStockCount>()
                     .To<StockCounter>()
                     .FromComponentsInHierarchy()
                     .AsCached();
            Container.Bind<ISlotRoleDeterminer>()
                     .To<SlotRoleDeterminer>()
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

            Container.Bind<ISlotPrizeOrderer>()
                     .To<SlotPrizeOrderer>()
                     .AsTransient();
        }
    }
}