using System;

namespace MedalPusher.Utils
{
    public static partial class ObservableEx
    {
        private static readonly Action<Exception> OnErrorNone = e => throw e;
        private static readonly Action OnCompletedNone = () => { };

        private struct ActionValuePair<T>
        {
            public ActionValuePair(Action<T> action, T value)
            {
                this.Action = action;
                this.Value = value;
            }
            public Action<T> Action { get; }
            public T Value { get; }
        }
    }
}