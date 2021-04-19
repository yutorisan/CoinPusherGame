using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using MedalPusher.Item.Pool;
using MedalPusher.Lottery;
using MedalPusher.Slot.Internal;
using UnityEngine;
using Zenject;

namespace MedalPusher
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMedalPayoutOperation>()
                     .To<MedalPayoutOperator>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IFieldItemPayoutOperation>()
                     .To<FieldItemPayoutOperator>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IObservableMedalPoolInfo>()
                     .To<MedalPool>()
                     .FromComponentInHierarchy()
                     .AsCached();

            Container.Bind<IObservableLotteryStatus>()
                     .To<LotteryBowlRotater>()
                     .FromComponentInHierarchy()
                     .AsCached();
            Container.Bind<IReadOnlyObservableSlotProdctionStatus>()
                     .To<SlotDriver>()
                     .FromComponentInHierarchy()
                     .AsCached();
        }
    }
}