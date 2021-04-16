using Zenject;

namespace MedalPusher.Input
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputProvider>()
                     .To<UserInputProvider>()
                     .AsCached();
            Container.Bind(typeof(IGameCommandProvider), typeof(IInitializable))
                     .To<GameCommandProvider>()
                     .AsCached();

        }
    }
}