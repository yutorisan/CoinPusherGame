using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
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
        Container.Bind<IMedalPayoutOperator>()
                 .To<MedalPayouter>()
                 .FromComponentInHierarchy()
                 .AsCached();


    }
}