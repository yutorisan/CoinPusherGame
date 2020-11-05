using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using MedalPusher.Item.Payout.Pool;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private GameObject m_winMedalChecker;

    public override void InstallBindings()
    {
        Container.Bind<IObservableItemChecker<IMedal>>()
                 .To<MedalPusher.Item.Checker.MedalChecker>()
                 .FromComponentOn(m_winMedalChecker)
                 .AsCached();
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