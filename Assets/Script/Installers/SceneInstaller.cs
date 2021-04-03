using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IMedalPayoutOperation>()
                 .To<MedalPayouterStorage>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<IObservableMedalPoolInfo>()
                 .To<MedalPool>()
                 .FromComponentInHierarchy()
                 .AsCached();
    }
}