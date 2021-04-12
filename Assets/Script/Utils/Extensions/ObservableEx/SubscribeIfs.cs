using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UniRx;
using UnityEditor;

namespace MedalPusher.Utils
{
    public static partial class ObservableEx
    {
        public static IDisposable SubscribeIfs<T>(this IObservable<T> source, SubscribeIfParams<T> onNexts)
        {
            return SubscribeIfs(source, onNexts, OnErrorNone, OnCompletedNone);
        }
        public static IDisposable SubscribeIfs<T>(this IObservable<T> source, SubscribeIfParams<T> onNexts, Action<Exception> onError)
        {
            return SubscribeIfs(source, onNexts, onError, OnCompletedNone);
        }
        public static IDisposable SubscribeIfs<T>(this IObservable<T> source, SubscribeIfParams<T> onNexts, Action onCompleted)
        {
            return SubscribeIfs(source, onNexts, OnErrorNone, onCompleted);
        }
        public static IDisposable SubscribeIfs<T>(this IObservable<T> source, SubscribeIfParams<T> onNexts, Action<Exception> onError, Action onCompleted)
        {
            return new SubscribeIfsOperator<T>(source, onNexts).Subscribe(avp => avp.Action(avp.Value), onError, onCompleted);
        }

        private class SubscribeIfsOperator<T> : IObservable<ActionValuePair<T>>
        {
            private IObservable<T> m_observable;
            private SubscribeIfParams<T> m_switchParams;
            public SubscribeIfsOperator(IObservable<T> source, SubscribeIfParams<T> switchParams)
            {
                m_observable = source;
                m_switchParams = switchParams;
            }

            public IDisposable Subscribe(IObserver<ActionValuePair<T>> observer)
            {
                return m_observable.Subscribe(new SubscribeIfsObserver(m_switchParams, observer));
            }

            private class SubscribeIfsObserver : IObserver<T>
            {
                private SubscribeIfParams<T> m_ifs;
                private IObserver<ActionValuePair<T>> m_observer;
                private object lockObj = new object();

                public SubscribeIfsObserver(SubscribeIfParams<T> switchParams, IObserver<ActionValuePair<T>> observer)
                {
                    m_ifs = switchParams;
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
                            foreach (var ifStatement in m_ifs)
                            {
                                //どのケースにあたるかをひとつずつチェック
                                if (ifStatement.Condition(value))
                                {
                                    //最初にあたったケースのアクションを発行して終了
                                    m_observer.OnNext(new ActionValuePair<T>(ifStatement.OnNextAction, value));
                                    return;
                                }
                            }
                            //どのケースにも当てはまらなかった場合、default句が定義されている場合はそのアクションを発行
                            if (m_ifs.IsExistElse)
                            {
                                m_observer.OnNext(new ActionValuePair<T>(m_ifs.ElseAction, value));
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

    public class ElseIfStatement<T>
    {
        public ElseIfStatement(Predicate<T> condition, Action<T> onNext)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (onNext == null) throw new ArgumentNullException(nameof(onNext));
            this.Condition = condition;
            this.OnNextAction = onNext;
        }
        public Predicate<T> Condition { get; }
        public Action<T> OnNextAction { get; }
    }

    public class SubscribeIfParams<T> : IEnumerable<ElseIfStatement<T>>
    {
        private IEnumerable<ElseIfStatement<T>> m_ifStatements;

        /// <summary>
        /// elseなしバージョン
        /// </summary>
        /// <param name="ifs"></param>
        public SubscribeIfParams(params ElseIfStatement<T>[] ifs)
        {
            m_ifStatements = ifs;
        }
        /// <summary>
        /// elseありバージョン
        /// </summary>
        /// <param name="elseAction"></param>
        /// <param name="ifs"></param>
        public SubscribeIfParams(Action<T> elseAction, params ElseIfStatement<T>[] ifs) : this(ifs)
        {
            this.ElseAction = elseAction;
        }

        /// <summary>
        /// else句のアクション
        /// </summary>
        public Action<T> ElseAction { get; }
        /// <summary>
        /// else句が存在するか
        /// </summary>
        public bool IsExistElse => ElseAction != null;

        public IEnumerator<ElseIfStatement<T>> GetEnumerator()
        {
            return m_ifStatements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_ifStatements).GetEnumerator();
        }
    }
}