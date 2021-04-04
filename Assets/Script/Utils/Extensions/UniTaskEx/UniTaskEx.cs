using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public static class UniTaskEx
{
    public static UniTask ContinueWithIf(this UniTask task, bool condition, Func<UniTask> continueTask)
    {
        if (!condition) return task;
        else return task.ContinueWith(continueTask);
    }
    public static UniTask WhenAll(this IEnumerable<UniTask> source) => UniTask.WhenAll(source);
}
