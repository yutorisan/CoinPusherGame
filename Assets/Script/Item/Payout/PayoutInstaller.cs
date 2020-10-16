using MedalPusher.Item.Payout;
using MedalPusher.Item.Payout.Pool;
using UnityEngine;
using Zenject;

public class PayoutInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IMedalPool>()
                 .To<MedalPool>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<IObservableMedalPayouter>()
                 .To<MedalPayouter>()
                 .FromComponentInHierarchy()
                 .AsCached();
    }
}