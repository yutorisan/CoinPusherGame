using MedalPusher.GameSystem.Facade;
using MedalPusher.Item;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

public class FacadeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IInitialInstantiateMedalCountSetterFacade>()
                 .To<MedalPool>()
                 .FromComponentInHierarchy()
                 .AsCached();
        Container.Bind<IInitialFillMedalCountSetterFacade>()
                 .To<MedalFieldAutoFiller>()
                 .FromComponentInHierarchy()
                 .AsCached();
    }
}