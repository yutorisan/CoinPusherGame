using MedalPusher.Slot.Stock;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    public class SlotInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IObservableStockCount>()
                     .To<StockCounter>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IObservableSlotStatus>()
                     .To<Slot>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<ISlotRoleDeterminer>()
                     .To<SlotRoleDeterminer>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<ISlotProductionDeterminer>()
                     .To<SlotProductionDeterminer>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IReelDriver>()
                     .To<ReelDriver>()
                     .FromComponentInHierarchy()
                     .AsCached();
        }
    }
}