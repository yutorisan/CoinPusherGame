using UnityEngine;
using System.Collections;
using UniRx;
using System;


public class NewMonoBehaviour : MonoBehaviour
{
    private IDisposable m_disposable;
    // Use this for initialization
    void Start()
    {
        //m_disposable =
        //    Observable.Interval(TimeSpan.FromSeconds(0.1))
        //              .Take(30)
        //              .SubscribeIfs(new SubscribeIfParams<long>(
        //                  n => print($"{n}は奇数"),
        //                  new ElseIfStatement<long>(n => n % 2 == 0, n => print($"{n}は偶数")),
        //                  new ElseIfStatement<long>(n => n % 3 == 0, n => print($"{n}は3の倍数")),
        //                  new ElseIfStatement<long>(n => n % 5 == 0, n => print($"{n}は5の倍数")),
        //                  new ElseIfStatement<long>(n => n % 7 == 0, n => print($"{n}は7の倍数")),
        //                  new ElseIfStatement<long>(n => n % 11 == 0, n => print($"{n}は11の倍数"))),
        //                  e => print("次のエラーが発生しました:" + e.Message),
        //                  () => print("OnCompleted"));

        //Observable.Throw<int>(new Exception("エラーのテスト"))
        //        .SubscribeIfs<int>(new SubscribeIfParams<int>(
        //                  n => print($"{n}は奇数"),
        //                  new ElseIfStatement<int>(n => n % 2 == 0, n => print($"{n}は偶数")),
        //                  new ElseIfStatement<int>(n => n % 3 == 0, n => print($"{n}は3の倍数")),
        //                  new ElseIfStatement<int>(n => n % 5 == 0, n => print($"{n}は5の倍数")),
        //                  new ElseIfStatement<int>(n => n % 7 == 0, n => print($"{n}は7の倍数")),
        //                  new ElseIfStatement<int>(n => n % 11 == 0, n => print($"{n}は11の倍数"))),
        //                  e => print("次のエラーが発生しました:" + e.Message),
        //                  () => print("OnCompleted"));

        //Observable.Interval(TimeSpan.FromSeconds(0.1))
        //          .SubscribeIfs(new SubscribeIfParams<long>(), () => print("OnCompleted"));
        Observable.Interval(TimeSpan.FromSeconds(0.1))
            .Select(l => l % 5)
            .SubscribeSwitch(new SubscribeSwitchParams<long>(
                new Case<long>(0, n => print($"{n}は5の倍数です")),
                new Case<long>(1, n => print($"{n}を5で割ると1余ります")),
                new Case<long>(2, n => print($"{n}を5で割ると2余ります")),
                new Case<long>(3, n => print($"{n}を5で割ると3余ります")),
                new Case<long>(4, n => print($"{n}を5で割ると4余ります"))),
                e => print("次のエラーが発生しました:" + e.Message),
                () => print("OnCompleted"));

        //Observable.IntervalFrame(10)

    }

    //[ContextMenu("Dispose")]
    //private void Dispose()
    //{
    //    m_disposable.Dispose();
    //}
}
