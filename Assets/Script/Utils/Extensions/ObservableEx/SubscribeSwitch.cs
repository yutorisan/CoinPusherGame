using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;

namespace MedalPusher.Utils
{
    public static partial class ObservableEx
    {
        public static IDisposable SubscribeSwitch<T>(this IObservable<T> source, SubscribeSwitchParams<T> onNexts)
        {
            return SubscribeSwitch(source, onNexts, OnErrorNone, OnCompletedNone);
        }
        public static IDisposable SubscribeSwitch<T>(this IObservable<T> source, SubscribeSwitchParams<T> onNexts, Action<Exception> onError)
        {
            return SubscribeSwitch(source, onNexts, onError, OnCompletedNone);
        }
        public static IDisposable SubscribeSwitch<T>(this IObservable<T> source, SubscribeSwitchParams<T> onNexts, Action onCompleted)
        {
            return SubscribeSwitch(source, onNexts, OnErrorNone, onCompleted);
        }
        public static IDisposable SubscribeSwitch<T>(this IObservable<T> source, SubscribeSwitchParams<T> onNexts, Action<Exception> onError, Action onCompleted)
        {
            return new SubscribeSwitchOperator<T>(source, onNexts).Subscribe(avp => avp.Action(avp.Value), onError, onCompleted);
        }

        private class SubscribeSwitchOperator<T> : IObservable<ActionValuePair<T>>
        {
            private IObservable<T> m_observable;
            private SubscribeSwitchParams<T> m_switchParams;
            public SubscribeSwitchOperator(IObservable<T> source, SubscribeSwitchParams<T> switchParams)
            {
                m_observable = source;
                m_switchParams = switchParams;
            }

            public IDisposable Subscribe(IObserver<ActionValuePair<T>> observer)
            {
                return m_observable.Subscribe(new SubscribeSwitchObserver(m_switchParams, observer));
            }

            private class SubscribeSwitchObserver : IObserver<T>
            {
                private SubscribeSwitchParams<T> m_switchParams;
                private IObserver<ActionValuePair<T>> m_observer;
                private object lockObj = new object();

                public SubscribeSwitchObserver(SubscribeSwitchParams<T> switchParams, IObserver<ActionValuePair<T>> observer)
                {
                    m_switchParams = switchParams;
                    m_observer = observer;
                }

                public void OnCompleted()
                {
                    lock (lockObj)
                    {
                        m_observer.OnCompleted();
                        m_observer = null;
                    }
                }

                public void OnError(Exception error)
                {
                    lock (lockObj)
                    {
                        m_observer.OnError(error);
                        m_observer = null;
                    }
                }

                public void OnNext(T value)
                {
                    lock (lockObj)
                    {
                        if (m_observer == null) return;
                        try
                        {
                            foreach (var swi in m_switchParams)
                            {
                                //どのケースにあたるかをひとつずつチェック
                                if (swi.Value.Equals(value))
                                {
                                    //最初にあたったケースのアクションを発行して終了
                                    m_observer.OnNext(new ActionValuePair<T>(swi.Action, value));
                                    return;
                                }
                            }
                            //どのケースにも当てはまらなかった場合、default句が定義されている場合はそのアクションを発行
                            if (m_switchParams.IsExistDefault)
                            {
                                m_observer.OnNext(new ActionValuePair<T>(m_switchParams.DefaultAction, value));
                            }
                        }
                        catch (Exception ex)
                        {
                            m_observer.OnError(ex);
                            m_observer = null;
                        }
                    }
                }
            }
        }
    }

    public class Case<T>
    {
        public Case(T caseValue, Action<T> caseAction)
        {
            if (caseValue == null) throw new ArgumentNullException(nameof(caseValue));
            if (caseAction == null) throw new ArgumentNullException(nameof(caseAction));
            this.Value = caseValue;
            this.Action = caseAction;
        }
        public T Value { get; }
        public Action<T> Action { get; }
    }

    public class SubscribeSwitchParams<T> : IEnumerable<Case<T>>
    {
        private IEnumerable<Case<T>> m_cases;

        /// <summary>
        /// defaultなしバージョン
        /// </summary>
        /// <param name="cases"></param>
        public SubscribeSwitchParams(params Case<T>[] cases)
        {
            this.m_cases = cases;
        }
        /// <summary>
        /// defalutありバージョン
        /// </summary>
        /// <param name="defaultAction"></param>
        /// <param name="cases"></param>
        public SubscribeSwitchParams(Action<T> defaultAction, params Case<T>[] cases) : this(cases)
        {
            this.DefaultAction = defaultAction;
        }

        public Action<T> DefaultAction { get; }
        public bool IsExistDefault => DefaultAction != null;

        public IEnumerator<Case<T>> GetEnumerator()
        {
            return m_cases.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_cases).GetEnumerator();
        }
    }
}