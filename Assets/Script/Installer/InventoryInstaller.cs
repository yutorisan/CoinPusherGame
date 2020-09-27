using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

public class InventoryInstaller : MonoInstaller
{
    [SerializeField]
    private GameObject m_winMedalChecker;

    public override void InstallBindings()
    {
        Container.Bind<IObservableItemChecker<IMedal>>()
                 .To<MedalPusher.Item.Checker.MedalChecker>()
                 .FromComponentOn(m_winMedalChecker)
                 .AsTransient();
        Container.Bind<IObservableMedalInventory>()
         .To<MedalInventory>()
         .FromComponentInHierarchy()
         .AsTransient();
        Container.Bind<IMedalPayoutOperator>()
                 .To<MedalPayouter>()
                 .FromComponentInHierarchy()
                 .AsTransient();
        Container.Bind<IMedalPool>()
                 .To<MedalPool>()
                 .FromComponentInHierarchy()
                 .AsTransient();

        //NotMonoInstaller.Install(Container);

    }
}