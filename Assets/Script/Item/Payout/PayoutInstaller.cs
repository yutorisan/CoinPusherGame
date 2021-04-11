using MedalPusher.Item.Payout;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

public class PayoutInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IMedalPoolPickUper>()
                 .To<MedalPool>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<IObservableMedalPayoutStock>()
                 .To<MedalPayouterStorage>()
                 .FromComponentInHierarchy()
                 .AsCached();
    }
}