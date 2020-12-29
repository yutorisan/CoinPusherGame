using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;

public class InventoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObservableMedalInventory>()
                 .To<MedalInventory>()
                 .FromComponentInHierarchy()
                 .AsCached();

    }
}