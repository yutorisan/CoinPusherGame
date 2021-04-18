using MedalPusher.Item.Checker;
using MedalPusher.Lottery;
using UnityEngine;
using Zenject;

public class LotteryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IBallBornOperator>()
                 .To<BallBorner>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<IObservableBallBorned>()
                 .To<BallBorner>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind(typeof(IObservableBallCount), typeof(IInitializable))
                 .To<OnLotteryBallCounter>()
                 .AsCached();

        LotteryPrizeCollector lotteryPrizeCollector = new LotteryPrizeCollector();
        Container.Bind<ILotteryPrizeInsertionSlot>()
                 .To<LotteryPrizeCollector>()
                 .FromInstance(lotteryPrizeCollector)
                 .AsCached();
        Container.Bind<ILotteryResultSubmitter>()
                 .To<LotteryPrizeCollector>()
                 .FromInstance(lotteryPrizeCollector)
                 .AsCached();
    }
}