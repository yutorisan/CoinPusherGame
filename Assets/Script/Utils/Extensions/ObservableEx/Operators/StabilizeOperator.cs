using System;
using UniRx;
using UniRx.Operators;

namespace MedalPusher.Utils
{
    //public static partial class ObservableEx
    //{
    //    public static IObservable<T> Stabilize<T>(this IObservable<T> source, TimeSpan waitTime)
    //    {
    //        var connectable = source.Publish();

    //        var a =
    //            connectable.SelectMany(x => connectable)
    //                       .Timeout(waitTime)
    //                       .Catch((TimeoutException e) => connectable);
    //    }
    //    public static IObservable<T> StabilizeFrame<T>(this IObservable<T> source, int waitFrame)
    //    {

    //    }

    //    private class StabilizeObservable<T> : OperatorObservableBase<T>
    //    {
    //        private readonly IObservable<T> source;
    //        private readonly int waitFrame;

    //        public StabilizeObservable(IObservable<T> source, int waitFrame) : base(source.IsRequiredSubscribeOnCurrentThread())
    //        {
    //            this.source = source;
    //            this.waitFrame = waitFrame;
    //        }

    //        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
    //        {
    //            return source.Subscribe(new Stabilize(this, observer, cancel));
    //        }

    //        private class Stabilize : OperatorObserverBase<T, T>
    //        {
    //            private readonly StabilizeObservable<T> parent;
                

    //            public Stabilize(StabilizeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
    //                : base(observer, cancel)
    //            {
    //            }

    //            public override void OnCompleted()
    //            {
    //                throw new NotImplementedException();
    //            }

    //            public override void OnError(Exception error)
    //            {
    //                throw new NotImplementedException();
    //            }

    //            public override void OnNext(T value)
    //            {
    //                throw new NotImplementedException();
    //            }
    //        }
    //    }
    //}
}