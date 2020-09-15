using UnityEngine;
using Zenject;

public class DITestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IHelloWorlder>()
                 .To<HelloWorlder>()
                 .FromMethod(() => new HelloWorlder("parameter"))
                 .AsTransient();
        Container.Bind<IDITestInjectComponent>()
                 .To<DITestInjectComponent>()
                 .FromComponentInChildren()
                 .AsTransient();
    }
}