using System;
using UniRx;
using UnityEngine;
using UnityUtility;

public static class InputObservable
{
    public static IObservable<Unit> FromKeyCode(KeyCode key)
    {
        return Observable.EveryUpdate()
                         .Where(_ => Input.GetKey(key))
                         .AsUnitObservable();
    }

    public static IObservable<KeyCode> FromAnyKey()
    {
        var observable = Observable.Empty<KeyCode>();

        foreach (var keycode in EnumExtensions.GetEnumValues<KeyCode>())
        {
            observable = observable.Merge(FromKeyCode(keycode).Select(_ => keycode));
        }

        return observable;
    }
}
