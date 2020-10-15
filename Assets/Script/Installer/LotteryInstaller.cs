using MedalPusher.Lottery;
using UnityEngine;
using Zenject;

public class LotteryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObservableLotteryRotater>()
                 .To<LotteryBowlRotater>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<ILotteryPrizeInsertionSlot>()
                 .To<LotteryPrizeCollector>()
                 .FromComponentInHierarchy()
                 .AsCached();
    }
}