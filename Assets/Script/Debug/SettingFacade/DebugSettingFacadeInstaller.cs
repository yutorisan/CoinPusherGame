using MedalPusher.Item;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

namespace MedalPusher.Debug
{
    public class DebugSettingFacadeInstaller : MonoInstaller
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
}