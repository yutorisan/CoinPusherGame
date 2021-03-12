using System;
using UniRx;

public static partial class ObservableEx
{
    public static IObservable<T> ExcludeNull<T>(this IObservable<T> source) =>
        source.Where(n => n != null);
}
