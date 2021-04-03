using UnityEngine;
using Zenject;
using MedalPusher.Input;

public class InputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameCommandProvider>()
                 .To<UserInputProvider>()
                 .AsCached();
    }
}