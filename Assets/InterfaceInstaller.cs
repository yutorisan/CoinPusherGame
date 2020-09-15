using MedalPusher.Item.Payout;
using UnityEngine;
using Zenject;

public class InterfaceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObservableMedalPayouter>()
                 .To<MedalPayouter>()
                 .FromComponentInHierarchy()
                 .AsSingle();
    }
}