using Zenject;

namespace MedalPusher.Input
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameCommandProvider>()
                     .To<GameCommandProvider>()
                     .AsCached();
        }
    }
}